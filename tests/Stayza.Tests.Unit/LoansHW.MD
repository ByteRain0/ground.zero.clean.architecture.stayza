# ✅ Homework: Unit Tests for `BookCopy` Aggregate

## 🎯 Objective

Write unit tests for the `BookCopy` aggregate to ensure it behaves correctly under various scenarios and maintains its invariants.

---

## 🧪 Methods to Test

- `Return()`
- `FulfillReservation()`
- `CancelReservation()`
- `Retire()`

---

## ✅ Acceptance Criteria

1. **Invariant Coverage**
    - Ensure all aggregate invariants are validated and enforced.
    - Confirm the aggregate transitions only to valid states.

2. **Exception Handling**
    - Cover all expected exceptions under invalid or edge-case scenarios.

3. **Domain Events**
    - Assert that the correct domain events are published.
    - Check event type or content as appropriate for each method.

4. **Recommended Testing Style**
    - Follow the **Arrange-Act-Assert** pattern.
    - Use consistent test naming similar to the one defined in class.

---