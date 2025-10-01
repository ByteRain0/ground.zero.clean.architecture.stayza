using Stayza.Core.Messaging;

namespace Stayza.Tests.Integration.Mocks;

public class MockProducer : IMessageProducer
{

    private readonly MockEventsStore _mockEventsStore;

    public MockProducer(MockEventsStore mockEventsStore)
    {
        _mockEventsStore = mockEventsStore;
    }

    public void PublishMessage(Message message, string routingKey)
    {
        _mockEventsStore.DomainEvents.Add(message);
    }
}