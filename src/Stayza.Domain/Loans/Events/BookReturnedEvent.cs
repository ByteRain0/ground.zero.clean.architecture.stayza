using Stayza.Core.Entity;
using Stayza.Core.Messaging;

namespace Stayza.Domain.Loans.Events;

public record BookReturnedEvent(
    Guid BookCopyId,
    Guid LoanId,
    DateTimeOffset ReturnDate) : IDomainEvent
{
    public string RoutingKey => RoutingKeys.BookCopyReturnedTopic
        .ReplaceBookCopyIdPlaceholderWith(BookCopyId.ToString())
        .ReplaceLoanIdPlaceholderWith(LoanId.ToString());
}