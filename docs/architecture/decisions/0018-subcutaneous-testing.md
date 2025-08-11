# 18. Subcutaneous Testing for Background Jobs and Internal Processes

Date: 2025-08-11

## Status
Accepted

## Context
Certain system behaviors, such as scheduled cron jobs and background processing tasks, execute “under the surface” without direct API endpoints or user interaction.  
Testing these internal processes requires a different approach from typical API integration tests.

Challenges include:
- Exercising background jobs and internal logic that runs outside of HTTP request pipelines.
- Maintaining realistic infrastructure dependencies such as database and messaging systems.
- Isolating tests and ensuring repeatability in a complex asynchronous environment.

## Decision
We will implement **subcutaneous tests** to cover background jobs and internal workflows with the following approach:

- Use **Testcontainers** to provide isolated instances of infrastructure dependencies (PostgreSQL, RabbitMQ).
- Use **`WebApplicationFactory`** to spin up the application host, providing full DI container and scoped service lifetimes.
- Obtain required services (e.g., cron job classes, job schedulers, message publishers) directly from the **service scope** of the `WebApplicationFactory`.
- Trigger job execution methods manually within the test to simulate scheduled runs or on-demand invocation.
- Use scoped repositories and infrastructure services from the DI container to verify side effects and outcomes of job execution.
- Reset state between tests using database cleanup tools (e.g., Respawn) and messaging queue purging to ensure test isolation.

## Consequences
- **Easier:** Allows testing of internal application logic and scheduled tasks without relying on external triggers or complex orchestration.
- **Easier:** Leverages the existing application DI and infrastructure, improving test realism and reliability.
- **Easier:** Provides fast feedback on critical background processes that might otherwise go untested until runtime.
- **Harder:** Tests require careful setup to properly isolate state and dependencies.
- **Harder:** Some background job behaviors may be time-dependent or asynchronous, requiring test synchronization strategies.
- **Risk:** Over-reliance on manual triggering in tests may miss certain edge cases that happen only during real schedules.