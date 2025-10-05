using Stayza.Core.Messaging;

namespace Stayza.Tests.EndToEnd;

public class MockProducer : IMessageProducer
{
    public void PublishMessage(Message message, string routingKey)
    {
        return;
    }
}