using Stayza.Core.Messaging;
using Message = Stayza.Core.Messaging.Message;

namespace Stayza.Tests.EndToEnd;

public class MockProducer : IMessageProducer
{
    public void PublishMessage(Message message, string routingKey)
    {
        return;
    }
}