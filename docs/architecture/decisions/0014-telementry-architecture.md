# 14. Usage of OpenTelemetry for telemetry data collection and observability

Date: 2025-08-11

## Status
Accepted

## Context
To achieve comprehensive observability, the system needs to collect telemetry data including traces, metrics, and logs.

Requirements include:
- Collecting distributed traces to understand request flows and performance bottlenecks.
- Collecting metrics for dashboards and alerting.
- Logging human-readable messages, such as audit trails and errors.
- Efficiently filtering and sampling telemetry data to reduce overhead and storage costs.
- Flexibility to create custom spans and add contextual information programmatically within the codebase.
- Exporting telemetry data to a centralized OpenTelemetry Collector for further processing and integration with monitoring backends.

## Decision
We will implement observability using **OpenTelemetry (OTel)** with the following approach:

- Instrument the application codebase to create **custom spans** and add attributes for detailed trace context where appropriate.
- Use OpenTelemetry SDK to automatically capture traces, metrics, and logs.
- Configure **sampling** strategies and **filters** to control telemetry data volume and focus on relevant traces.
- Export telemetry data to an **OpenTelemetry Collector**, which will forward data to backend systems (e.g., Jaeger, Prometheus, Loki, Grafana).
- Use **logs** primarily for human-readable audit messages and error details.
- Use **traces and spans** to provide technical, detailed insights into request flow and debugging information.
- Use **metrics** for real-time monitoring, dashboards, and alerting.

## Consequences
- **Easier:** Improved visibility into system performance and behavior through structured telemetry data.
- **Easier:** Efficient control of telemetry data volume and quality through sampling and filtering.
- **Easier:** Separation of concerns by using logs for human-readable messages and traces/metrics for technical analysis.
- **Harder:** Requires initial setup and ongoing tuning of sampling and filtering to balance observability and performance overhead.
- **Harder:** Developers need to learn how to instrument code with custom spans and use OTel APIs effectively.
- **Risk:** Misconfiguration can lead to loss of important telemetry or excessive overhead. Continuous monitoring and adjustment are needed.  
