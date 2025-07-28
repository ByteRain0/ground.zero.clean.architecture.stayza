using Stayza.Core.Entity;
using Stayza.Core.Messaging;

namespace Stayza.Domain.Loans.Events;

public record ReservationCancelledEvent(
    Guid BookCopyId,
    Guid ReservationId,
    Guid UserId,
    string Reason)
    : IDomainEvent
{
    public string RoutingKey => RoutingKeys.BookCopyReservationCancelledTopic
        .ReplaceBookCopyIdPlaceholderWith(BookCopyId.ToString())
        .ReplaceReservationIdPlaceholderWith(ReservationId.ToString());
}