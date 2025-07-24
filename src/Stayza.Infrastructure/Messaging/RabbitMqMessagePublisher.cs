using Stayza.Core.AsyncProcessing;
using Stayza.Core.Entity;

namespace Stayza.Infrastructure.Messaging;

public class RabbitMqMessagePublisher : IPublisher
{
    public Task Publish(IDomainEvent domainEvent)
    {
        return Task.CompletedTask;
    }
}