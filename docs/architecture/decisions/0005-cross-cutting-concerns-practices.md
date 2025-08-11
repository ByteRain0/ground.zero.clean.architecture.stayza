# 5. Cross-cutting concerns in the API

Date: 2025-08-11

## Status
Accepted

## Context
The API needs a consistent, maintainable approach for implementing cross-cutting concerns such as:
- Activity tracing for distributed diagnostics.
- Performance monitoring and metrics collection.
- Request and response logging.
- Validation of incoming requests.

Requirements:
- Minimize boilerplate code in controllers and endpoints.
- Ensure concerns are applied consistently across all API requests.
- Leverage built-in ASP.NET Core capabilities to reduce custom implementation.
- Maintain flexibility for future extensions or replacements.

## Decision
We will implement cross-cutting concerns using ASP.NET Core’s **built-in middlewares** and **filters**:
- **Middlewares** will handle concerns that apply to all requests (e.g., activity tracing with `Activity`/OpenTelemetry, request logging, performance timing).
- **Filters** will handle concerns at the endpoint level (e.g., model validation, authorization checks, custom action filters for specific features).

## Consequences
- **Easier:** Consistent application of cross-cutting concerns without duplicating logic in controllers.
- **Easier:** Clear separation between core business logic and infrastructural concerns.
- **Easier:** Built-in middlewares and filters reduce maintenance overhead and leverage Microsoft-supported patterns.
- **Harder:** Ordering of middlewares and filters becomes more critical; misconfiguration can lead to missed logs or skipped validations.
- **Risk:** Overuse of filters/middlewares for heavy operations could introduce performance overhead if not monitored.  
