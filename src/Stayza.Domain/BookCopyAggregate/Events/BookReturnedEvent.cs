using Stayza.Core.Entity;

namespace Stayza.Domain.BookCopyAggregate.Events;

public record BookReturnedEvent(
    Guid BookCopyId,
    Guid LoanId,
    DateTimeOffset ReturnDate) : IDomainEvent;