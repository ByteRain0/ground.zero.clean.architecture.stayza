using Stayza.Core.Entity;
using Stayza.Domain.BookCopyAggregate.Events;
using Stayza.Domain.BookCopyAggregate.Exceptions;

namespace Stayza.Domain.BookCopyAggregate;

public class BookCopy : AggregateRoot
{
    public Guid BookId { get; set; }

    private readonly List<Reservation> _reservations = new();

    private Loan? _currentLoan;

    public bool IsAvailable => _currentLoan == null || _currentLoan.IsReturned;

    public bool IsLoaned => _currentLoan != null && !_currentLoan.IsReturned;

    public BookCopy(Guid id, Guid bookId)
        : base(id)
    {
        BookId = bookId;
    }

    public void Reserve(
        Guid userId,
        DateTimeOffset utcNow)
    {
        if (!IsAvailable)
            throw new BookNotAvailableForReservation();

        if (_reservations.Any(r => r.UserId == userId && r.Status == ReservationStatus.Active))
            throw new ReservationAlreadyExistsException(userId);

        _reservations.Add(new Reservation(
            userId: userId,
            reservedAt: utcNow,
            bookCopyId: Id,
            id: Guid.NewGuid()));
    }

    public void StartLoan(
        Guid userId,
        DateTimeOffset utcNow)
    {
        if (IsLoaned)
            throw new BookCopyAlreadyLoanedException(userId);

        var hasReservation = _reservations.Any(r =>
            r.UserId == userId &&
            r.Status == ReservationStatus.Active);

        if (!hasReservation)
            throw new ReservationNotFound();

        _currentLoan = new Loan(
            copyId: Id,
            userId: userId,
            loanDate: utcNow,
            dueDate: utcNow.AddDays(14),
            id: Guid.NewGuid());

        AddDomainEvent(new BookLoanedEvent(
            BookCopyId: Id,
            UserId: userId,
            LoanId: _currentLoan.Id,
            LoanDate: _currentLoan.LoanDate,
            DueDate: _currentLoan.DueDate));
    }

    public void Return(
        Guid userId,
        DateTimeOffset utcNow)
    {
        if (!IsLoaned)
            throw new InvalidOperationException("Copy is not loaned.");

        if (_currentLoan!.UserId != userId)
            throw new InvalidOperationException("Cannot return not owned book");

        _currentLoan!.MarkAsReturned(returnedAt: utcNow);

        AddDomainEvent(new BookReturnedEvent(
            BookCopyId: Id,
            LoanId: _currentLoan.Id,
            ReturnDate: utcNow));
    }

    public void ExpireReservation(
        Guid reservationId,
        DateTimeOffset utcNow)
    {
        var reservation = _reservations.SingleOrDefault(x => x.Id == reservationId);

        if (reservation is null)
            throw new ReservationNotFound();

        if (utcNow > reservation.ExpiresAt)
            throw new InvalidOperationException("Cannot expire valid reservation");

        reservation.Status = ReservationStatus.Expired;

        AddDomainEvent(new ReservationExpiredEvent(
            ReservationId: reservation.Id,
            UserId: reservation.UserId,
            BookCopyId: Id));
    }

    public void FulFillReservation(
        Guid reservationId,
        DateTimeOffset utcNow)
    {
        var reservation = _reservations.SingleOrDefault(x => x.Id == reservationId);

        if (reservation is null)
            throw new ReservationNotFound();

        if (reservation.Status == ReservationStatus.Cancelled)
            throw new ReservationAlreadyCancelledException();

        reservation.Status = ReservationStatus.Fulfilled;

        reservation.ExpiresAt = utcNow.AddDays(3);

        AddDomainEvent(new ReservationFulfilledEvent(
            ReservationId: reservation.Id,
            UserId: reservation.UserId,
            BookCopyId: Id,
            AvailableUntil: reservation.ExpiresAt));
    }

    public void CancelReservation(
        Guid reservationId,
        Guid userId)
    {
        var reservation = _reservations.SingleOrDefault(x => x.Id == reservationId);

        if (reservation is null)
            throw new ReservationNotFound();

        if (reservation.Status == ReservationStatus.Cancelled)
            throw new ReservationAlreadyCancelledException();

        if (reservation.UserId != userId)
            throw new InvalidOperationException("Cannot cancel reservation for invalid user");
        
        reservation.Status = ReservationStatus.Cancelled;
    }
}