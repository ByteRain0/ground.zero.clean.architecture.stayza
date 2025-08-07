using Stayza.Core.Entity;
using Stayza.Core.Messaging;

namespace Stayza.Domain.Loans.Events;

public record BookLoanedEvent(
    Guid BookCopyId,
    string UserId,
    Guid LoanId,
    DateTimeOffset LoanDate,
    DateTimeOffset DueDate) : IDomainEvent
{
    public string RoutingKey => RoutingKeys
        .BookCopyLoanedTopic
        .ReplaceBookCopyIdPlaceholderWith(BookCopyId.ToString());
}