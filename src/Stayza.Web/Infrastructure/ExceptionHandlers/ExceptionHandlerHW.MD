# ✅ Homework: Exception Handlers in a REST API

## 🎯 Objective

Implement and test a set of exception handlers for custom domain errors using ASP.NET Core's `IExceptionHandler` middleware and the `ProblemDetails` format.

---

## 🧪 Exceptions to Handle

Create and test exception handling for the following domain-specific exceptions from the `Books` and `Loans` aggregates

---

## ✅ Acceptance Criteria

1. **Correct mapping of exceptions to `ProblemDetails`**
    - Proper HTTP status codes (404, 400, 409, 403, etc.)
    - Fields like `Title`, `Detail`, `Status`, and `Type` are accurately populated.
    - Additional fields (e.g., `errors`) can be included to provide more context.

2. **Logging of relevant exceptions**
    - Critical errors should be logged with the appropriate log level (`Warning`, `Error`, etc.)

3. **Testing through real HTTP requests**
    - Ensure that the HTTP response matches the expected `ProblemDetails` structure.

---

## 🛠 Recommendations

- At a later stage in the course we will write integration tests for the custom errors.
- Alternatively you can have a single exception handler per aggregate root where you will switch through multiple exception types.

---