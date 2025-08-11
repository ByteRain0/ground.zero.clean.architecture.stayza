# 19. Contract Testing Between Stayza.API and Notifications API Using Pact.net

Date: 2025-08-11

## Status
Accepted

## Context
The Stayza.API integrates with an external Notifications API to send user notifications.  
To ensure both systems remain compatible despite independent development and deployment, we need a robust contract testing approach.

Challenges include:
- Avoiding integration failures caused by API contract mismatches.
- Verifying that Stayza.API sends requests conforming to Notifications API expectations.
- Allowing Notifications API to evolve without breaking Stayza.API’s assumptions.

## Decision
We will implement **consumer-driven contract testing** using **Pact.net** and **xUnit**:

- **Stayza.API acts as the consumer** and defines expectations for the Notifications API interactions using Pact contracts.
- Pact contracts specify the requests Stayza.API will send and the expected responses from the Notifications API.
- Use **xUnit** test framework to write automated contract tests in Stayza.API codebase validating these expectations.
- Pact.net generates and publishes contracts that the Notifications API provider can use to verify compliance.
- Contracts are versioned and stored alongside code, enabling CI pipelines to catch breaking changes early.
- Integration with CI/CD pipelines automates contract verification for both consumer and provider.

## Consequences
- **Easier:** Early detection of breaking contract changes, reducing runtime integration issues.
- **Easier:** Clear, executable documentation of integration points between Stayza.API and Notifications API.
- **Easier:** Supports independent development and deployment cycles while maintaining integration safety.
- **Harder:** Initial setup and maintenance overhead for contract tests and CI integration.
- **Harder:** Requires coordination with Notifications API team to integrate contract verification on their side.
- **Risk:** Over-reliance on contracts can miss behavioral or state-related integration issues beyond request/response schemas.  
