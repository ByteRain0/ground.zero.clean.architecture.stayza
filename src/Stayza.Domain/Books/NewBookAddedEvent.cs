using Stayza.Core.Entity;

namespace Stayza.Domain.Books;

public class NewBookAddedEvent : IDomainEvent
{
    public string RoutingKey => "book.added";
}