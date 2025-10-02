using System.Text.Json;
using Shouldly;
using Stayza.Tests.Integration.Mocks;

namespace Stayza.Tests.Integration.Base_V2;

public static class MockEventStoreExtensions
{
    public static TEvent GetPublishedEvent<TEvent>(this MockEventsStore eventsStore) where TEvent : class
    {
        var evt = eventsStore.DomainEvents
            .SingleOrDefault(e => e.Header.EventCode == typeof(TEvent).Name);

        evt.ShouldNotBeNull($"Expected event {typeof(TEvent).Name} to be published.");

        var deserialized = JsonSerializer.Deserialize<TEvent>(evt.Body);
        deserialized.ShouldNotBeNull($"Event body should be deserializable to {typeof(TEvent).Name}.");

        return deserialized;
    }    
}