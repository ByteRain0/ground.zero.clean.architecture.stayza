# 11. Integration Between Aggregate Roots and Eventual Consistency

Date: 2025-08-11

## Status
Accepted

## Context
In a domain-driven design, each aggregate root defines a transactional consistency boundary:
- Changes within a single aggregate are consistent and atomic.
- Different aggregates should not be directly coupled through shared transactions to avoid complexity and performance issues.

However, business processes often require coordination and data propagation between multiple aggregates, potentially across different subdomains or bounded contexts.

This raises the need to handle integration between aggregates in a way that:
- Respects aggregate boundaries and transactional consistency rules.
- Supports eventual consistency where immediate synchronous updates are not required or feasible.
- Enables asynchronous communication of state changes between aggregates.

## Decision
We will integrate aggregates by:

- Treating each aggregate root as an isolated transactional boundary, ensuring all state changes within it are atomic.
- Using **Domain Events** emitted by aggregates to signal relevant state changes or intentions to other parts of the system.
- Handling these domain events asynchronously via event handlers or message queues to update or trigger behavior in other aggregates, thereby achieving eventual consistency.
- Avoiding direct references or database transactions spanning multiple aggregates.
- Designing domain events carefully to convey intent and sufficient data for downstream processing without exposing internal aggregate details.

## Consequences
- **Easier:** Maintains strong transactional guarantees within aggregates while enabling cross-aggregate workflows.
- **Easier:** Improves system scalability and resilience by decoupling aggregates through asynchronous messaging.
- **Easier:** Domain events provide clear audit trails and help trace business processes.
- **Harder:** Clients must handle eventual consistency scenarios, including stale reads and reconciliation logic.
- **Harder:** Increased complexity in testing and debugging asynchronous workflows.
- **Risk:** Without proper monitoring and retries, event handling failures may cause data inconsistencies or delays.  
