using Stayza.Core.Entity;

namespace Stayza.Domain.Events;

public record LoanIsOverdueEvent(
    Guid LoanId, 
    DateTimeOffset TriggeredAt) : IDomainEvent;