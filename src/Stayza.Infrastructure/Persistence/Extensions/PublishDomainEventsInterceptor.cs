using Microsoft.EntityFrameworkCore.Diagnostics;
using Stayza.Core.Entity;
using Stayza.Core.Messaging;

namespace Stayza.Infrastructure.Persistence.Extensions;

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
        var domainEvents = context
            .ChangeTracker
            .Entries<AggregateRoot>()
            .Select(entry => entry.Entity)
            .SelectMany(aggregateRoot =>
            {
                var domainEvents = aggregateRoot.DomainEvents;
                aggregateRoot.ClearDomainEvents();
                return domainEvents;
            })
            .ToList();

        foreach (IDomainEvent domainEvent in domainEvents)
        {
            var header = new Header(
                sourceCode: "stayza_web",
                eventCode: domainEvent.GetType().Name);

            var message = new Message(header: header, body: domainEvent);
            
            _messageProducer.PublishMessage(message, domainEvent.RoutingKey);
        }
    }
}