# 17. Outside-In Integration Testing of the API with Infrastructure Dependencies

Date: 2025-08-11

## Status
Accepted

## Context
Outside-in integration testing aims to validate the system behavior end-to-end by exercising the API and all its infrastructure dependencies. 
Unlike unit tests, which isolate a small piece of logic, integration tests cover multiple layers and external systems, 
making them inherently more complex but crucial for confidence in production readiness.

The main challenges are:
- **Infrastructure setup:** The API depends on external systems such as PostgreSQL for persistence, RabbitMQ for messaging, and an external notifications service. Reliable testing requires realistic, isolated instances of these systems.
- **Test isolation:** Each test should run against a clean state to avoid flaky behavior caused by leftover data or messages.
- **Performance:** Integration tests tend to be slower than unit tests. Balancing speed with reliability and coverage is essential to maintain developer productivity.
- **Observability:** Detailed tracing and logging help diagnose issues when tests fail, especially when asynchronous messaging or external mocks are involved.

## Decision
To address these challenges, we will adopt the following approach for outside-in integration testing:

### 1. Spinning up the API for testing
- Use **`WebApplicationFactory<TEntryPoint>`** from ASP.NET Core testing libraries to start an in-memory instance of the Web API. This allows sending real HTTP requests to test the API end-to-end without deploying externally.

### 2. Infrastructure dependencies using Testcontainers
- Use **Testcontainers** (a .NET library for managing Docker containers in tests) to launch disposable instances of required infrastructure:
    - **PostgreSQL container**: A fresh, isolated database instance per test suite with network isolation.
    - **RabbitMQ container**: A message broker instance that can be configured, monitored, and reset between tests.
    - **WireMock container**: A mock HTTP server to simulate the external Notifications API, enabling request verification and controlled responses.

### 3. Configuration overrides
- Override the default configuration in `WebApplicationFactory` to redirect the API’s connection strings, messaging endpoints, and external service URLs to point to the test container instances and mocks. This ensures the API under test uses isolated test resources.

### 4. Test lifecycle management
- Implement **`IClassFixture<WebApplicationFactory>`** for each test class, allowing all tests in the class to share the API and infrastructure setup. This reduces overhead compared to spinning up and tearing down per test method, improving execution speed.
- Use **Respawn** to reset the PostgreSQL database between individual tests by rolling back changes or truncating tables, guaranteeing a clean state without full container restarts.

### 5. Verifying asynchronous messaging
- Create a **custom RabbitMQ consumer** within the test suite that subscribes to relevant queues and verifies that messages are published correctly during test execution. This helps assert eventual consistency scenarios and side effects from domain events.

### 6. Observability in tests
- Integrate **OpenTelemetry (OTel)** into the test environment to collect traces, metrics, and logs of test executions. This helps:
    - Visualize request flows including asynchronous operations.
    - Diagnose slow or failed tests with detailed context.
    - Correlate telemetry with test failures for rapid troubleshooting.

## Consequences

### Benefits
- **Realistic environment:** Tests run against actual services rather than mocks or stubs, increasing confidence that the system behaves correctly in production-like conditions.
- **Isolation and repeatability:** Disposable containers and database resets ensure tests do not interfere with each other, reducing flaky tests.
- **Speed optimizations:** Sharing infrastructure setup at the test class level and resetting DB state between tests optimizes execution time while maintaining isolation.
- **Comprehensive verification:** Testing message publication and external service calls validates side effects and integrations, which are often difficult to cover in unit tests.
- **Improved diagnostics:** OpenTelemetry integration provides rich observability, aiding root cause analysis.

### Challenges
- **Increased complexity:** The test setup is more complicated than unit testing, requiring knowledge of Docker, container orchestration, and test lifecycle management.
- **Maintenance overhead:** Testcontainers, mocks, and configuration overrides need to be kept in sync with production dependencies and updated when services evolve.
- **Resource usage:** Running multiple containers and the full API can increase resource consumption and may slow down CI pipelines if not managed carefully.
- **Potential flakiness:** External dependencies and network communications introduce possible points of failure that require robust retry and timeout strategies.

## Additional considerations
- **CI/CD integration:** Tests should be optimized for CI environments, need careful resource cleanup to prevent container leaks.
- **Extensibility:** The custom RabbitMQ consumer and WireMock setup should be modular and reusable to support additional integration scenarios as the system grows.
