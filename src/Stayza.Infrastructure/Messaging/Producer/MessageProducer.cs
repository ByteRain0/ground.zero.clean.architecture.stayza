using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Stayza.Core.Messaging;
using Stayza.Core.Telemetry;

namespace Stayza.Infrastructure.Messaging.Producer;

public class MessageProducer : IMessageProducer
{
    private readonly IModel _channel;
    private readonly RabbitMQSettings _rabbitSettings;
    private readonly ILogger<MessageProducer> _logger;

    public MessageProducer(
        RabbitMQSettings rabbitSettings, 
        IModel channel, 
        ILogger<MessageProducer> logger)
    {
        _rabbitSettings = rabbitSettings;
        _channel = channel;
        _logger = logger;
    }

    public void PublishMessage(Message message, string key)
    {
        using var publishActivity = RunTimeDiagnosticConfig.Source.StartActivity("RabbitMQ Publish");

        publishActivity.SetRoutingKey(key);
        
        if (publishActivity is not null)
        {
            message.Header.Properties?.Add("traceparent", publishActivity.Id);
        }
        
        var properties = _channel.CreateBasicProperties();
        properties.ContentType = "text/plain";
        properties.Headers = message.Header.Properties;
        
        var body = Encoding.UTF8.GetBytes(message.Body);

        try
        {
            _channel.BasicPublish(
                exchange: _rabbitSettings.ExchangeName,
                routingKey: key,
                basicProperties: properties,
                body: body);
            
            //TODO: can either add the message body as prop to the activity to witch the log binds.
            _logger.LogInformation("Published message with key {Key} {Message}", key, JsonSerializer.Serialize(message));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed publishing message");
            throw;
        }
    }
}