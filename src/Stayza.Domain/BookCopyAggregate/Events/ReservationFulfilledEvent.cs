using Stayza.Core.Entity;

namespace Stayza.Domain.BookCopyAggregate.Events;

public record ReservationFulfilledEvent(
    Guid ReservationId,
    Guid UserId,
    Guid BookCopyId,
    DateTimeOffset AvailableUntil) : IDomainEvent;