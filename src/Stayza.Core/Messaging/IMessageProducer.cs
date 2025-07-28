namespace Stayza.Core.Messaging;

public interface IMessageProducer
{
    void PublishMessage(Message message, string routingKey);
}