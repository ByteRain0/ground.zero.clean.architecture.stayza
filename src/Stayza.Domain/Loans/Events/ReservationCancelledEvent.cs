using Stayza.Core.Entity;

namespace Stayza.Domain.Loans.Events;

public record ReservationCancelledEvent(
    Guid ReservationId,
    Guid UserId,
    Guid BookCopyId,
    string Reason)
    : IDomainEvent;