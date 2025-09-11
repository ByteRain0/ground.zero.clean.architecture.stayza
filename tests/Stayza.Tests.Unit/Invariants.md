# Invariants

## Book copies
- A librarian can add new books and book copies.
- A book can have multiple copies.
- A book cannot have two same copies.
- A book copy cannot be removed once loaned.
- Removing a non existing copy should throw NotFound exception.

## Reservations
- The user should be able to reserve an available book.
- The user should be able to reserve a book that is loaned.
- The user should not be able to reserve the book once it is retired.
- The user can't have 2 reservations for the same book.
- Retiring a book should cancel all existing active reservations.
- Fufilling a reservation should notify user about book being available to loan.
- After fulling reservation the user has 3 days to loan the book or his reservation will be cancelled.

## Loans
- The user cannot loan a book without prior reservation.
- The user cannot loan an already loaned book. (Logic :P )
- Once loaned book copy reservation for user is removed.
- Loaning a book triggers an BookLoanedEvent for consumers.
- Returning a book copy triggers a BookReturnedEvent for consumers.
- Only the user who loaned the book is allowed to return it.
- Returning a not loaned book should not be possible.