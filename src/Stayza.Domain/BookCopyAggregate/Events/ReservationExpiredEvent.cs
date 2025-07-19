using Stayza.Core.Entity;

namespace Stayza.Domain.BookCopyAggregate.Events;

public record ReservationExpiredEvent(
    Guid ReservationId,
    Guid UserId,
    Guid BookCopyId) : IDomainEvent;