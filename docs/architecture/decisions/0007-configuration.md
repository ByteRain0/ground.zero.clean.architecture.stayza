# 7. Application configuration using appsettings

Date: 2025-08-11

## Status
Accepted

## Context
The application requires a flexible way to manage configuration settings such as connection strings, feature flags, API keys, and environment-specific values.

While there are multiple external configuration sources available (e.g., Spring Cloud Config, Azure App Configuration, AWS/GCP configuration services), adopting these would introduce additional infrastructure complexity and dependencies at this stage.

## Decision
We will use the built-in **appsettings.json** files for configuration management.

- Environment-specific configurations will be supported via files like `appsettings.Development.json`, `appsettings.Production.json`, etc.
- Configuration will be loaded using the standard ASP.NET Core configuration providers.
- No external or cloud-based configuration services will be used for now.

## Consequences
- **Easier:** Simple, file-based configuration that is easy to manage and version control.
- **Easier:** Values provided from appsettings can be over-written by environment variables whenever needed..
- **Easier:** No additional infrastructure or service dependencies required.
- **Harder:** Limited dynamic configuration updates without redeploying the app.
- **Risk:** Scaling to multiple environments or microservices may require revisiting this approach later to support centralized configuration.  
