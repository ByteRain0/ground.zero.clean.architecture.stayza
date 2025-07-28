namespace Stayza.Core.Entity;

public interface IDomainEvent
{
    public string RoutingKey { get; }
}