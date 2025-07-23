using Microsoft.EntityFrameworkCore.Diagnostics;
using Stayza.Core.AsyncProcessing;
using Stayza.Core.Entity;

namespace Stayza.Infrastructure.Persistence.Extensions;

public sealed class PublishDomainEventsInterceptor : SaveChangesInterceptor
{
    private readonly IPublisher _publisher;

    public PublishDomainEventsInterceptor(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            await PublishDomainEventsAsync(eventData.Context);
        }

        return result;
    }

    private async Task PublishDomainEventsAsync(Microsoft.EntityFrameworkCore.DbContext context)
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
            await _publisher.Publish(domainEvent);
        }
    }
}