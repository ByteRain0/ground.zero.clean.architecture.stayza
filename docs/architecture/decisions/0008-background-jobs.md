# 8. Background job processing with TickerQ

Date: 2025-08-11

## Status
Accepted

## Context
The application needs to run scheduled background jobs daily to perform maintenance tasks.  
Currently, we require two daily jobs:
- `NotifyUsersAboutExpiringBookReservations`
- `RemoveExpiredAndCancelledReservations`

Requirements for the background job system include:
- Lightweight and easy integration with our existing stack.
- First-party support for Entity Framework Core, which we already use.
- Simple, attribute-based job configuration.
- A UI dashboard to monitor jobs with configurable basic authentication.
- Ability to trigger jobs on demand manually.

Existing popular solutions like Hangfire or Quartz are heavier or require more setup.

## Decision
We will use **TickerQ**, a lightweight background job library with native EF Core integration.

Key points of this approach:
- Jobs are configured by decorating methods in services with an attribute, e.g.,  
  `[TickerFunction(nameof(NotifyUsersAboutExpiringReservations), "0 7 * * *")]`
- The cron expression controls the schedule (daily at 7 AM in the example).
- TickerQ provides a built-in UI dashboard secured via configurable basic auth.
- Supports manual triggering of jobs through the dashboard or API.

This fits our requirements for simplicity, EF Core integration, and ease of use.

## Consequences
- **Easier:** Rapid setup and minimal boilerplate for scheduled background jobs.
- **Easier:** Consistent integration with EF Core data context and transactions.
- **Easier:** Monitoring and manual job triggering via the built-in dashboard.
- **Harder:** Limited ecosystem compared to more mature solutions like Hangfire or Quartz, which may affect advanced scenarios.
- **Risk:** Potential for future limitations if job complexity or scaling requirements grow beyond TickerQ’s capabilities, possibly requiring migration later.  
