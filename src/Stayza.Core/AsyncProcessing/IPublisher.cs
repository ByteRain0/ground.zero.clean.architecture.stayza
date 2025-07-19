using Stayza.Core.Entity;

namespace Stayza.Core.AsyncProcessing;

public interface IPublisher
{
    Task Publish(IDomainEvent domainEvent);
}