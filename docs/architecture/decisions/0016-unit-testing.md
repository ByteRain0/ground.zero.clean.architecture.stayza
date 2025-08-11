# 16. Unit Testing Domain Aggregate Roots and System Architecture

Date: 2025-08-11

## Status
Accepted

## Context
Ensuring domain correctness and system quality requires thorough automated testing.

Key goals for unit testing domain aggregate roots include:
- Verifying business invariants and rules enforced by aggregates.
- Confirming correct publishing of domain events from aggregates.
- Using factories and constant values to build repeatable, consistent test data.
- Ensuring tests are low level, fast to execute, and provide quick feedback to developers.

Additionally, architectural unit tests help verify the overall structure and boundaries of the system (e.g., dependency rules, layering).

## Decision
We will implement:

- **Domain aggregate root unit tests** focusing on:
    - Testing business invariants and behaviors directly on aggregates.
    - Verifying that domain events are published as expected.
    - Using test data factories and constant seed values to standardize test inputs.
- Tests designed to be **fast**, isolated, and easy to write and maintain, enabling rapid developer feedback.
- **Architectural unit tests** that validate constraints such as layer dependencies and module boundaries to prevent architectural drift.

## Consequences
- **Easier:** Early detection of domain logic errors and invariant violations.
- **Easier:** High developer confidence due to fast and reliable test suite.
- **Easier:** Consistent test data improves test reliability and maintainability.
- **Easier:** Architectural tests help enforce system modularity and design principles over time.
- **Harder:** Requires investment in writing comprehensive domain and architectural tests upfront.
- **Risk:** Poorly designed tests can become brittle or slow, reducing effectiveness—careful test design is necessary.  
- **Risk:** Unit tests should be used as only a fast feedback loop, most of the value from testing will be from the outside-in integration and e2e testing.  

