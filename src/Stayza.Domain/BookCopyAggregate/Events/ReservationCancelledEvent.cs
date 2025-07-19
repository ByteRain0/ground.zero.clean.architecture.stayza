using Stayza.Core.Entity;

namespace Stayza.Domain.BookCopyAggregate.Events;

public record ReservationCancelledEvent(
    Guid ReservationId,
    Guid UserId,
    Guid BookCopyId)
    : IDomainEvent;