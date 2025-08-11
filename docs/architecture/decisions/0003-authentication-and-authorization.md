# 3. Authentication and authorization

Date: 2025-08-11

## Status
Accepted

## Context

The application requires a secure and reliable way to handle user authentication and authorization.  
We need a solution that:
- Integrates seamlessly with ASP.NET Core.
- Supports common authentication flows (registration, login, password management).
- Manages user roles and claims for fine-grained access control.
- Minimizes custom implementation effort while following best security practices.

Available options considered:
- Custom authentication/authorization logic.
- Third-party identity providers (Auth0, Okta, etc.).
- ASP.NET Core Identity with Entity Framework Core.

## Decision
We will use **ASP.NET Core Identity** with **Entity Framework Core** for our authentication and authorization setup.

This approach was chosen because:
- It is officially supported and maintained by Microsoft.
- It provides a ready-to-use, extensible identity system with minimal setup.
- It integrates seamlessly with EF Core for persisting user and role data.
- It supports modern security features like password hashing, token-based authentication, and role-based access control out-of-the-box.

## Consequences
- **Easier:** Rapid implementation of a secure and feature-rich identity system.
- **Easier:** Built-in support for roles, claims, two-factor authentication, and password resets.
- **Harder:** Some customization may require extending default Identity models and stores.
- **Risk:** Tightly coupling authentication logic to EF Core may limit flexibility if we switch to a different persistence layer in the future.  
