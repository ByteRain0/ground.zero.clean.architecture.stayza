# 20. Health Checks Implementation in Stayza.API

Date: 2025-08-11

## Status
Accepted

## Context
To ensure operational reliability and support automated monitoring, Stayza.API needs comprehensive health checks that:
- Confirm the API’s own availability and responsiveness.
- Verify connectivity and readiness of critical infrastructure dependencies such as PostgreSQL, RabbitMQ, and external services (e.g., Notifications API).
- Provide actionable status data consumable by monitoring and orchestration systems (e.g., Kubernetes, Prometheus, or external alerting tools).
- Are lightweight and performant to avoid impacting API performance or scalability.

## Decision
We will implement health checks in Stayza.API using the following approach:

- Use ASP.NET Core’s built-in **Health Checks middleware** to expose health endpoints (e.g., `/alive` and `/health`).
- Include **liveness checks** to confirm that the API process is running and responsive.
- Include **readiness checks** that verify:
    - Successful connection to the PostgreSQL database via lightweight queries or connection tests.
    - Connection and ability to publish/consume messages on RabbitMQ.
    - Reachability of the Notifications API endpoint (using a lightweight HTTP call or TCP check).
- Implement custom health check classes for dependencies that are not covered by built-in checks or require specific logic.
- Configure health check responses to comply with standards (e.g., JSON formatted, HTTP 200 on success, HTTP 503 on failure).
- Optionally integrate health check results with OpenTelemetry metrics and tracing to enhance observability.
- Secure health endpoints appropriately, exposing readiness and liveness only to authorized monitoring systems.

## Consequences
- **Easier:** Provides real-time operational insight into Stayza.API and its dependencies.
- **Easier:** Enables automated orchestration platforms to perform rolling upgrades and failover based on health status.
- **Easier:** Early detection of degraded states before impacting users.
- **Harder:** Requires maintenance of health check implementations as dependencies evolve.
- **Risk:** Overly complex or slow health checks could impact API performance or lead to false alarms if not carefully designed.  
