# 15. Integration via Domain Events with Notifications REST API

Date: 2025-08-11

## Status
Accepted

## Context
The system needs to notify users about important events (e.g., reservation expiry) via an external notifications service.  
We want to keep domain logic decoupled from external communication concerns while ensuring timely notifications.

## Decision
We will integrate the notifications REST API using the following approach:

- Domain events capturing relevant business occurrences will be published after successful transactions.
- An event handler, implementing the `IListener` interface, will listen for these domain events asynchronously.
- This event handler will invoke the external notifications REST API to send user notifications.
- The handler will use scoped dependencies (e.g., HTTP clients) created via `IServiceScopeFactory` to ensure proper resource management.
- This keeps the domain layer free from direct integration code, preserving separation of concerns.

## Consequences
- **Easier:** Decouples domain logic from external notification infrastructure.
- **Easier:** Supports asynchronous, reliable notification delivery.
- **Easier:** Enables independent scaling and evolution of the notification system.
- **Harder:** Requires robust error handling and retry policies in event handlers to ensure notifications are delivered.
- **Risk:** Potential delays between domain event occurrence and notification delivery due to asynchronous processing.  
