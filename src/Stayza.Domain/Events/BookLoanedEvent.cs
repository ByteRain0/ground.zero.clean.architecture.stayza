using Stayza.Core.Entity;

namespace Stayza.Domain.Events;

public record BookLoanedEvent(
    Guid BookCopyId,
    Guid UserId,
    Guid LoanId,
    DateTimeOffset LoanDate,
    DateTimeOffset DueDate) : IDomainEvent;