# 2. Client app needs an easy way to integrate with the backend API

Date: 2025-08-11

## Status
Accepted

## Context
The frontend application requires a straightforward and reliable way to communicate with the backend API.

Potential integration approaches considered:
- **REST**
- **GraphQL**
- **gRPC**

## Decision
We will develop a **REST** interface for the API.

This approach was chosen because:
- The development team has strong familiarity with REST, reducing ramp-up time and delivery risk.
- REST is widely supported, well-documented, and fits our integration needs.
- **gRPC** is better suited for service-to-service communication rather than client-to-backend integration.
- **GraphQL** is a viable alternative but has a steeper learning curve and additional tooling overhead that we cannot accommodate at this stage.

## Consequences
- We will leverage Microsoft’s built-in support for building REST APIs.
- Given the lightweight nature of the application, we will implement the interface using **Minimal APIs** to reduce boilerplate and simplify development.  
