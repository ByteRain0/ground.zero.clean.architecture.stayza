using Stayza.Core.Entity;

namespace Stayza.Domain.BookCopyAggregate.Events;

public record LoanIsOverdueEvent(
    Guid LoanId,
    DateTimeOffset TriggeredAt) : IDomainEvent;