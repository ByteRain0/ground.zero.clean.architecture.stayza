using Stayza.Core.Entity;

namespace Stayza.Domain.BookCopyAggregate.Exceptions;

public record ReservationCancelledEvent(
    Guid ReservationId,
    Guid UserId,
    Guid BookCopyId)
    : IDomainEvent;