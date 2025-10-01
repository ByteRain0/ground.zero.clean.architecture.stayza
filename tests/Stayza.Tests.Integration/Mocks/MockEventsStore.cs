using Stayza.Core.Messaging;

namespace Stayza.Tests.Integration.Mocks;

public class MockEventsStore
{
    public List<Message> DomainEvents { get; set; }
}