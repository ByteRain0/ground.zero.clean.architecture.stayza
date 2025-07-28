using Stayza.Core.Entity;
using Stayza.Core.Messaging;

namespace Stayza.Domain.Loans.Events;

public record ReservationFulfilledEvent(
    Guid BookCopyId,
    Guid ReservationId,
    Guid UserId,
    DateTimeOffset AvailableUntil) : IDomainEvent
{
    public string RoutingKey => RoutingKeys.BookCopyReservationFulfilledTopic
        .ReplaceBookCopyIdPlaceholderWith(BookCopyId.ToString())
        .ReplaceReservationIdPlaceholderWith(ReservationId.ToString());
}