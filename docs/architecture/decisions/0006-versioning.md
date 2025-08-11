# 6. API versioning and OpenAPI/Swagger integration

Date: 2025-08-11

## Status
Accepted

## Context
The API will evolve over time, and changes may require breaking compatibility with existing clients.  
To manage this, we need a clear strategy for API versioning that:
- Allows multiple API versions to coexist.
- Is discoverable by API consumers.
- Works seamlessly with API documentation tooling (OpenAPI/Swagger).
- Fits naturally into the .NET 9 Minimal APIs and MVC-based controllers.

## Decision
We will use **ASP.NET Core API Versioning** package to implement versioning, with the following approach:
- Versioning will be applied via **URL segments** (e.g., `/api/v1/resource`) as the primary method.
- Support for **query string** and **header-based** versioning will be possible for flexibility but not the default.
- Controllers/endpoints will explicitly specify their version via `[ApiVersion]` attributes or `MapToApiVersion` for Minimal APIs.

For documentation:
- We will integrate **Swashbuckle.AspNetCore** (Swagger) with **API versioning** support to generate separate OpenAPI documents per version.
- Each version’s endpoints will be clearly grouped in Swagger UI with proper labels (e.g., “v1”, “v2”).
- The OpenAPI spec will be available for each version to allow client code generation and automated testing.

## Consequences
- **Easier:** Clients can migrate to new API versions at their own pace without breaking existing integrations.
- **Easier:** Swagger UI provides clear visibility of available versions and endpoints.
- **Easier:** Integration with OpenAPI allows code generation tools to target specific versions.
- **Harder:** Maintaining multiple API versions will increase complexity and testing effort.
- **Risk:** Without a clear deprecation policy, outdated API versions may linger longer than desired, increasing maintenance burden.  
