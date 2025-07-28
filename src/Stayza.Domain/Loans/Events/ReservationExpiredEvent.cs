using Stayza.Core.Entity;
using Stayza.Core.Messaging;

namespace Stayza.Domain.Loans.Events;

public record ReservationExpiredEvent(
    Guid BookCopyId,
    Guid ReservationId,
    Guid UserId) : IDomainEvent
{
    public string RoutingKey => RoutingKeys.BookCopyReservationExpiredTopic
        .ReplaceBookCopyIdPlaceholderWith(BookCopyId.ToString())
        .ReplaceReservationIdPlaceholderWith(ReservationId.ToString());
}