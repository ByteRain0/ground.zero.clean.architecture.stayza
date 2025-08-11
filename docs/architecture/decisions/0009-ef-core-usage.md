# 9. Using EF Core in a DDD + Clean Architecture scenario

Date: 2025-08-11

## Status
Accepted

## Context
The application follows a Domain-Driven Design (DDD) and Clean Architecture approach, emphasizing:
- Rich domain models encapsulating business logic.
- Clear separation of concerns between domain, infrastructure, and application layers.
- Need for robust persistence aligned with domain concepts.

EF Core is chosen as the ORM for data persistence. We want to align its usage with DDD best practices while leveraging EF Core’s features effectively.

## Decision
We will use EF Core with the following practices:

- **Rich domain models:** Entities and value objects encapsulate behavior, not just data.
- **Entity configurations:** Use `IEntityTypeConfiguration<TEntity>` implementations in the infrastructure layer to configure entity mappings and relationships, keeping domain models persistence-ignorant.
- **Data seeding:** Seed initial data via EF Core migrations using the `HasData` method in model configurations, ensuring consistent initial state.
- **Interceptors:** Implement EF Core SaveChanges interceptors to hook into persistence lifecycle, enabling functionalities like : domain event publishing after successful transactions.
- **Code-first approach:** Design domain models first, then create and apply migrations reflecting model changes.
- **Repositories:** Implement repositories in the infrastructure layer to abstract EF Core DbContext usage and provide aggregate root access per DDD principles.
- **Migrations on startup:** Automatically apply pending migrations during application startup in development and test environments to streamline iterative development.
- **Exiting integrations:** Authn/z and Background jobs will integrate easily with the existing EF Core infrastructure.
- **Value Object mapping:** Configure EF Core to map Value Objects as owned entity types or via custom conversions as appropriate.

## Consequences
- **Easier:** Strong alignment between domain models—including Value Objects—and persistence while preserving domain purity.
- **Easier:** Centralized configuration and seeding improve maintainability and reproducibility of data states.
- **Easier:** Domain events can be published reliably in SaveChanges interceptors, supporting event-driven architectures.
- **Easier:** Code-first and migrations provide a smooth iterative workflow for evolving the database schema.
- **Harder:** Requires discipline to keep domain models persistence-ignorant and avoid anemic models.
- **Harder:** Careful design needed to avoid performance pitfalls when using rich models with complex relationships.
- **Risk:** Applying migrations automatically on startup in non-production environments is safe, but care must be taken to avoid unintended schema changes in production.  
