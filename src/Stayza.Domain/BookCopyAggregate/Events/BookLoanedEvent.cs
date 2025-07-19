using Stayza.Core.Entity;

namespace Stayza.Domain.BookCopyAggregate.Events;

public record BookLoanedEvent(
    Guid BookCopyId,
    Guid UserId,
    Guid LoanId,
    DateTimeOffset LoanDate,
    DateTimeOffset DueDate) : IDomainEvent;