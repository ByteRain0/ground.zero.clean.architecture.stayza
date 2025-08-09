using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Stayza.Core.Entity;
using Stayza.Core.Messaging;
using Stayza.Core.Telemetry;

namespace Stayza.Infrastructure.Persistence.Interceptors;

public sealed class PublishDomainEventsInterceptor : SaveChangesInterceptor
{
    private readonly IMessageProducer _messageProducer;

    public PublishDomainEventsInterceptor(IMessageProducer messageProducer)
    {
        _messageProducer = messageProducer;
    }

    public override ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        { 
            PublishDomainEventsAsync(eventData.Context);
        }

        return ValueTask.FromResult(result);
    }

    private void PublishDomainEventsAsync(Microsoft.EntityFrameworkCore.DbContext context)
    {
        using var activity = RunTimeDiagnosticConfig.Source.StartActivity("Checking for events to publish");
        
        var domainEvents = context
            .ChangeTracker
            .Entries<AggregateRoot>()
            .Select(entry => entry.Entity)
            .SelectMany(aggregateRoot =>
            {
                var domainEvents = aggregateRoot.DomainEvents.ToList();
                aggregateRoot.ClearDomainEvents();
                return domainEvents;
            })
            .ToList();

        activity?.SetTag("countOfEventsToPublish", domainEvents.Count);
        
        foreach (IDomainEvent domainEvent in domainEvents)
        {
            using var subActivity =
                RunTimeDiagnosticConfig.Source.StartActivity($"Publishing {domainEvent.GetType().Name}");

            subActivity?.SetTag("RoutingKey", domainEvent.RoutingKey);
            
            var header = new Header(
                sourceCode: "stayza_web",
                eventCode: domainEvent.GetType().Name);

            var message = new Message(header: header, body: domainEvent);
            
            _messageProducer.PublishMessage(message, domainEvent.RoutingKey);
            
            // Example of how you can manually stop an activity if you need to
            // technically is redundant since the activity is bound to the scope
            subActivity?.Stop();
        }
    }
}