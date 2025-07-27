using Stayza.Core.Entity;

namespace Stayza.Domain.Loans.Events;

public record ReservationExpiredEvent(
    Guid ReservationId,
    Guid UserId,
    Guid BookCopyId) : IDomainEvent;