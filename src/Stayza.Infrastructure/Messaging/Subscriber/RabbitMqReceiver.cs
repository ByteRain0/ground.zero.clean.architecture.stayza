using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Stayza.Core.Messaging;
using Stayza.Core.Telemetry;

namespace Stayza.Infrastructure.Messaging.Subscriber;

public class RabbitMqReceiver
{
    private readonly RabbitMQSettings _rabbitSettings;
    private readonly IModel? _channel;
    private readonly List<IListener> _listeningServices;

    public RabbitMqReceiver(
        IServiceProvider sp,
        RabbitMQSettings rabbitSettings,
        IModel channel)
    {
        _listeningServices = sp.GetServices<IListener>().ToList();
        _rabbitSettings = rabbitSettings;
        _channel = channel;
    }

    private void Listener(IListener service)
    {
        _channel.ExchangeDeclare(
            exchange: _rabbitSettings.ExchangeName,
            type: _rabbitSettings.ExchangeType
        );

        var queueName = _channel.QueueDeclare(
            queue: _rabbitSettings.AppPrefix + Guid.NewGuid()).QueueName;

        _channel.QueueBind(
            queue: queueName,
            exchange: _rabbitSettings.ExchangeName,
            routingKey: service.RoutingKey);

        var consumerAsync = new AsyncEventingBasicConsumer(_channel);
        consumerAsync.Received += async (_, ea) =>
        {
            var parentContext = PropagateContextFromRabbitHeaders(ea.BasicProperties);
            using var activity = RunTimeDiagnosticConfig
                .Source
                .StartActivity("RabbitMQ Consume", ActivityKind.Consumer, parentContext);

            var body = ea.Body.ToArray();
            var message = new Message(
                header: new Header {Properties = ea.BasicProperties.Headers},
                serializedObject: Encoding.UTF8.GetString(body));

            await service.ProcessMessage(message, ea.RoutingKey);
            _channel?.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume(
            queue: queueName,
            autoAck: false,
            consumer: consumerAsync);
    }

    public void Dispose()
    {
        try
        {
            if (_channel == null)
            {
                Console.WriteLine("Warning: Channel was already disposed or not initialized");
                return;
            }
            _channel.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Critical: Error encountered disposing of rabbitmq receiver exceptions : {ex.StackTrace}");
        }
        

    }

    public void RegisterListeners()
    {
        _listeningServices.ForEach(Listener);
    }

    private static ActivityContext PropagateContextFromRabbitHeaders(IBasicProperties props)
    {
        if (props.Headers != null && props.Headers.TryGetValue("traceparent", out var traceParentObj))
        {
            var traceParent = Encoding.UTF8.GetString((byte[]) traceParentObj);
            var ctx = ActivityContext.Parse(traceParent, null);
            return ctx;
        }

        return default;
    }
}