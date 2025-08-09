using Ardalis.GuardClauses;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Stayza.Tests.Integration.Base;

public class RabbitMqTestMessageConsumer
{
    private readonly ConnectionFactory _factory;

    public RabbitMqTestMessageConsumer(string connectionString)
    {
        _factory = new ConnectionFactory
        {
            Uri = new Uri(connectionString)
        };
    }

    public async Task<Task<bool>> BindAndConsumeAsyncV2(
        string exchangeName,
        string routingKey,
        TimeSpan timeout,
        string testName = "na")
    {
        Guard.Against.NullOrEmpty(exchangeName);
        Guard.Against.NullOrEmpty(routingKey);

        var messageReceived = new TaskCompletionSource<bool>();

        var task = Task.Run(async () =>
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(
                exchange: exchangeName,
                type: ExchangeType.Topic);

            var queueName = channel.QueueDeclare(queue: $"consumer.for.{testName.ToLowerInvariant()}").QueueName;

            channel.QueueBind(
                queue: queueName,
                exchange: exchangeName,
                routingKey: routingKey);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (_, _) => { messageReceived.TrySetResult(true); };

            channel.BasicConsume(
                queue: queueName,
                autoAck: true,
                consumer: consumer);

            // await Task.Delay(TimeSpan.FromSeconds(60));
            
            var timeoutTask = Task.Delay(timeout);
            var completedTask = await Task.WhenAny(messageReceived.Task, timeoutTask);

            return completedTask == messageReceived.Task;
        });

        return task;
    }
}