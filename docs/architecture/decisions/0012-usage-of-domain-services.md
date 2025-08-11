# 12. Usage of Domain Services in the Domain Layer

Date: 2025-08-11

## Status
Accepted

## Context
In Domain-Driven Design, **Domain Services** represent operations or business logic that:
- Don’t naturally belong to a single entity or value object, or
- Involve coordination across multiple aggregates or domain concepts.

However, overusing domain services can indicate a design smell where aggregates or entities are not well defined or responsibilities are misplaced.

Our design goal is to:
- Keep business logic encapsulated within entities and aggregates as much as possible.
- Minimize the need for domain services by proper aggregate decomposition and modeling.

## Decision
We will:
- Prefer to place business rules and invariants within entities and aggregate roots, maintaining **rich domain models**.
- Avoid creating domain services unless a clear, well-justified scenario arises that cannot be naturally expressed within aggregates or entities.
- Regularly review domain model boundaries and responsibilities to reduce reliance on domain services.
- When domain services are necessary, keep them focused, stateless, and clearly separated from domain entities.

## Consequences
- **Easier:** Rich domain models improve encapsulation, testability, and expressiveness of the domain layer.
- **Easier:** Reduced cognitive overhead by limiting the number of domain services and distributing logic close to the data.
- **Harder:** Requires careful upfront modeling and iterative refinement of aggregates and entities.
- **Harder:** Some complex business logic might be more challenging to place, requiring thoughtful design trade-offs.
- **Risk:** Ignoring the need for domain services where they truly belong can lead to overloading aggregates or forcing awkward models.  
