# 10. Use of Rich Domain Models over Anemic Domain Models

Date: 2025-08-11

## Status
Accepted

## Context
In designing the domain layer, there is a choice between:
- **Anemic Domain Models**, where entities are simple data containers and business logic is implemented in services.
- **Rich Domain Models**, where entities and aggregates encapsulate both data and behavior, enforcing business rules internally.

Our architecture requires:
- A clear, maintainable place to express and enforce business invariants.
- Improved testability by focusing unit tests on domain behavior rather than procedural service logic.
- Support for **Domain Events** within aggregate roots to model side effects and eventual consistency.
- Structuring the domain into multiple subdomains with clear aggregate boundaries for scalability and clarity.

## Decision
We will adopt **Rich Domain Models** that:
- Encapsulate business logic and invariants inside entities and aggregates rather than exposing mutable data freely.
- Use aggregate roots as transactional consistency boundaries, responsible for maintaining their own invariants.
- Raise **Domain Events** from aggregates to communicate important state changes to other parts of the system asynchronously.
- Organize the domain into well-defined **subdomains**, each with its own aggregates and bounded contexts, promoting modularity and team alignment.
- Facilitate writing focused, behavior-driven unit tests directly on domain models without needing to mock extensive service logic.

## Consequences
- **Easier:** Business rules and invariants are centralized inside domain objects, reducing duplication and errors.
- **Easier:** Unit testing focuses on domain behavior, improving test coverage and confidence.
- **Easier:** Domain events enable clean separation of concerns and support for eventual consistency patterns.
- **Harder:** Requires careful modeling and discipline to avoid overcomplicating domain objects or aggregate boundaries.
- **Harder:** Learning curve for developers unfamiliar with rich domain modeling concepts.
- **Risk:** Structure relying only on unit tests is not sufficient.
- **Risk:** Poor aggregate design can lead to performance bottlenecks or transactional conflicts, requiring thoughtful design and iterative refinemen
