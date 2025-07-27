using Stayza.Core.Entity;

namespace Stayza.Domain.Loans.Events;

public record ReservationFulfilledEvent(
    Guid ReservationId,
    Guid UserId,
    Guid BookCopyId,
    DateTimeOffset AvailableUntil) : IDomainEvent;