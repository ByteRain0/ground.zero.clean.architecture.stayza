using Stayza.Core.Entity;
using Stayza.Core.Messaging;

namespace Stayza.Domain.Loans.Events;

public record LoanIsOverdueEvent(
    Guid BookCopyId,
    Guid LoanId,
    DateTimeOffset TriggeredAt) : IDomainEvent
{
    public string RoutingKey => RoutingKeys.BookCopyLoanOverdueTopic
        .ReplaceBookCopyIdPlaceholderWith(BookCopyId.ToString())
        .ReplaceLoanIdPlaceholderWith(LoanId.ToString());
}