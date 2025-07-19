using Stayza.Core.Entity;

namespace Stayza.Domain.Events;

public record BookReturnedEvent(
    Guid BookCopyId,
    Guid LoanId,
    DateTimeOffset ReturnDate) : IDomainEvent;