using Stayza.Core.Messaging;

namespace Stayza.MigrationService.Stubs;

public class MessageProducerStub : IMessageProducer
{
    public void PublishMessage(Message message, string routingKey)
    {
        return;
    }
}