using Stayza.Core.Messaging;

namespace Stayza.Tests.Subcutaneous.Stubs;

public class MessagePublisherStub : IMessageProducer
{
    public void PublishMessage(Message message, string routingKey)
    {
        return;
    }
}