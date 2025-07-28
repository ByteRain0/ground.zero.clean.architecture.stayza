using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Stayza.Core.Messaging;

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
        var properties = _channel.CreateBasicProperties();
        properties.ContentType = "text/plain";
        properties.Headers = message.Header.Properties;

        var body = Encoding.UTF8.GetBytes(message.Body);
        
        _channel.BasicPublish(
            exchange: _rabbitSettings.ExchangeName,
            routingKey: key,
            basicProperties: properties,
            body: body);

        _logger.LogInformation("Published message with key {Key} {Message}", key, message);
    }
}