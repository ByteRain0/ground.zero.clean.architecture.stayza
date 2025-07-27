using Stayza.Core.Entity;

namespace Stayza.Domain.Loans.Events;

public record LoanIsOverdueEvent(
    Guid LoanId,
    DateTimeOffset TriggeredAt) : IDomainEvent;