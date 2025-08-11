# 4. Exception handling in the API

Date: 2025-08-11

## Status
Accepted

## Context
The API needs a consistent and maintainable strategy for handling exceptions.  
Requirements:
- Centralized error handling that works across the entire API.
- Ability to chain multiple exception-handling middlewares to address different concerns (e.g., domain errors, validation errors, unhandled exceptions).
- Provide clients with clear and standardized error responses.
- Log errors for monitoring and diagnostics.
- Return a correlation or trace identifier to help link API errors to logs/traces.

Standards and references:
- [RFC 7807](https://datatracker.ietf.org/doc/html/rfc7807) Problem Details for HTTP APIs.

## Decision
We will implement exception handling using ASP.NET Core’s **`IExceptionHandler`** interface, with multiple middlewares chained together:
1. Specialized middlewares for known error types (e.g., validation, domain exceptions).
2. A generic "catch-all" middleware at the end of the chain to handle unhandled exceptions.

Middlewares will:
- Format responses using **ProblemDetails** according to RFC 7807.
- Log the exception with relevant context.
- Where possible include a **traceId** in the ProblemDetails response so the client can report it for debugging.

## Consequences
- **Easier:** Centralized and consistent error responses across the API.
- **Easier:** Logging and diagnostics are standardized, making it easier to correlate issues with traces.
- **Easier:** Adding or adjusting handling for specific exception types without changing unrelated parts of the pipeline.
- **Harder:** Slight increase in complexity due to multiple middlewares in the chain.
- **Risk:** If middleware order is incorrect, certain exceptions might bypass intended handlers. This will need to be addressed via careful configuration and testing.  
