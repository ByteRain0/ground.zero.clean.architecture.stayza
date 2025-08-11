# 13. Asynchronous communication using RabbitMQ and custom integration helpers

Date: 2025-08-11

## Status
Accepted

## Context
The system requires asynchronous communication for integrating domain events and external systems (e.g., notification services).

Key requirements and constraints include:
- Use of **domain events** stored in aggregate roots, published after EF Core commits changes.
- Avoid dependency on commercial or heavyweight messaging libraries like MassTransit or NServiceBus to reduce licensing costs and complexity.
- Maintain control and simplicity through a custom-built set of helper classes that integrate with **RabbitMQ** for message transport.
- Support flexible routing of messages using RabbitMQ **Topic Exchanges** and **routing keys** to decouple publishers and subscribers.
- Event handlers should be singletons, instantiated with `IServiceScopeFactory` to create scoped dependencies such as repositories or HTTP clients.
- Integration event handlers will bridge to external systems (e.g., notifications) reliably and asynchronously.

## Decision
We will implement asynchronous communication as follows:

- Domain events are collected within aggregate roots during transaction processing.
- After EF Core successfully saves changes, a **SaveChanges interceptor** will publish all domain events to RabbitMQ using our custom helper library.
- RabbitMQ **Topic Exchanges** will be used to route messages based on event types or categories, allowing fine-grained subscription filtering via **routing keys**.
- Event handlers implementing a custom `IListener` interface will consume and process messages selectively based on routing keys they subscribe to.
- Handlers are registered as **singleton services** and receive an `IServiceScopeFactory` in their constructors for scoped dependency resolution.
- Integration with external systems (e.g., notification services) will be encapsulated in dedicated event handlers listening to relevant domain or integration events.
- The custom helper library will abstract RabbitMQ connection management, publishing, subscription, and routing logic, providing a lightweight and maintainable messaging layer.

## Consequences
- **Easier:** Full control over the messaging infrastructure without licensing costs or heavy dependencies.
- **Easier:** Flexible and scalable message routing with Topic Exchanges and routing keys, improving decoupling.
- **Easier:** Clear separation between domain event publication and handling, supporting eventual consistency.
- **Harder:** Increased responsibility for maintaining and evolving the custom RabbitMQ integration code.
- **Harder:** Requires careful design of event schemas, routing keys, and error handling to ensure reliability and scalability.
- **Risk:** Potential for bugs or performance issues due to custom messaging implementation, requiring thorough testing and monitoring.  
