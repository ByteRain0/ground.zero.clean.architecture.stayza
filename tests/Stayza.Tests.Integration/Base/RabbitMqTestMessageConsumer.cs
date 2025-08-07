using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Stayza.Tests.Integration.Base;

public class RabbitMqTestMessageConsumer
{
    private readonly ConnectionFactory _factory;

    public RabbitMqTestMessageConsumer(string connectionString)
    {
        _factory = new ConnectionFactory()
        {
            Uri = new Uri(connectionString)
        };
    }

    public void BindQueue(
        string exchange,
        string routingKey = "")
    {
        using var connection = _factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(
            exchange: exchange,
            type: ExchangeType.Topic,
            durable: false);
        
        var queueName = channel.QueueDeclare().QueueName;
        
        var queueResult = channel.QueueDeclare(
            queue: queueName,
            durable: false,
            exclusive: false);

        channel.QueueBind(
            queue: queueResult.QueueName,
            exchange: exchange,
            routingKey: routingKey);
    }

    public async Task<bool> TryConsumeAsync(TimeSpan timeout)
    {
        var messageReceived = new TaskCompletionSource<bool>();
        using var connection = _factory.CreateConnection();
        using var channel = connection.CreateModel();

        var queueName = channel.QueueDeclare().QueueName;
        
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (_, _) => { messageReceived.SetResult(true); };

        channel.BasicConsume(
            queue: queueName, 
            autoAck: true, 
            consumer: consumer);

        var timeoutTask = Task.Delay(timeout);
        var completedTask = await Task.WhenAny(
            messageReceived.Task, 
            timeoutTask);

        return completedTask == messageReceived.Task;
    }
}