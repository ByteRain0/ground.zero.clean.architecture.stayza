using Stayza.Core.Entity;

namespace Stayza.Domain.Loans.Events;

public record BookReturnedEvent(
    Guid BookCopyId,
    Guid LoanId,
    DateTimeOffset ReturnDate) : IDomainEvent;