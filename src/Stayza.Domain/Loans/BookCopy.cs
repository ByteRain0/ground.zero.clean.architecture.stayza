using Ardalis.GuardClauses;
using Stayza.Core.Entity;
using Stayza.Core.Exceptions;
using Stayza.Domain.Loans.Events;
using Stayza.Domain.Loans.Exceptions;

namespace Stayza.Domain.Loans;

public class BookCopy : AggregateRoot
{
    public Guid BookId { get; set; }

    public bool IsRetired { get; private set; } = false;
    
    public Loan? CurrentLoan { get; private set; }

    public bool IsAvailable => CurrentLoan == null || CurrentLoan.IsReturned;

    public bool IsLoaned => CurrentLoan != null && !CurrentLoan.IsReturned;
    
    private readonly HashSet<Reservation> _reservations;
    
    public IReadOnlyCollection<Reservation> Reservations => _reservations.ToList();

    public IReadOnlyCollection<Reservation> PendingReservations =>
        _reservations.Where(r => r.Status == ReservationStatus.Pending).ToList();

    public Reservation? ActiveReservation => _reservations
        .SingleOrDefault(x => x.Status == ReservationStatus.Active);
    
    [Obsolete("Used only by ef core")]
    public BookCopy()
    {
        _reservations = new();
    }

    public BookCopy(Guid id, Guid bookId)
        : base(id)
    {
        BookId = bookId;
        _reservations = new();
    }

    public Reservation Reserve(
        string userId,
        DateTimeOffset utcNow)
    {
        // Either this in every method or add a test at IRepository level.
        Guard.Against.Null(_reservations);

        if (IsRetired)
            throw new BookNotAvailableForReservation();

        if (_reservations.Any(r => 
                r.UserId == userId && r.Status == ReservationStatus.Pending))
            throw new ReservationAlreadyExistsException(userId);

        var reservation = new Reservation(
            userId: userId,
            reservedAt: utcNow,
            bookCopyId: Id,
            id: Guid.NewGuid());

        _reservations.Add(reservation);

        return reservation;
    }

    public Loan StartLoan(
        string userId,
        DateTimeOffset utcNow)
    {
        Guard.Against.Null(_reservations);

        if (IsLoaned)
            throw new BookCopyAlreadyLoanedException(userId);

        var reservation = _reservations.SingleOrDefault(r =>
            r.UserId == userId &&
            r.Status == ReservationStatus.Pending);

        if (reservation is null)
            throw new EntityNotFoundException(
                entityType: nameof(Reservation),
                searchKey: $"userId: {userId}");

        _reservations.Remove(reservation);

        CurrentLoan = new Loan(
            bookCopyId: Id,
            userId: userId,
            timeRange: new TimeRange(
                start: utcNow,
                end: utcNow.AddDays(14)),
            id: Guid.NewGuid());

        AddDomainEvent(new BookLoanedEvent(
            BookCopyId: Id,
            UserId: userId,
            LoanId: CurrentLoan.Id,
            LoanDate: CurrentLoan.TimeRange.Start,
            DueDate: CurrentLoan.TimeRange.End));

        return CurrentLoan;
    }

    public Loan Return(
        string userId,
        DateTimeOffset utcNow)
    {
        Guard.Against.Null(CurrentLoan);

        if (!IsLoaned)
            throw new InvalidOperationException("Copy is not loaned.");

        if (CurrentLoan.UserId != userId)
            throw new InvalidOperationException("Cannot return not owned book");

        CurrentLoan!.MarkAsReturned(returnedAt: utcNow);

        AddDomainEvent(new BookReturnedEvent(
            BookCopyId: Id,
            LoanId: CurrentLoan.Id,
            ReturnDate: utcNow));

        return CurrentLoan;
    }
    
    public Reservation FulfillReservation(
        Guid reservationId,
        DateTimeOffset utcNow)
    {
        Guard.Against.Null(_reservations);

        var reservation = _reservations.SingleOrDefault(x => x.Id == reservationId);
        
        if (reservation is null)
            throw new EntityNotFoundException(
                entityType: nameof(Reservation),
                searchKey: reservationId.ToString());

        if (reservation.Status == ReservationStatus.Cancelled)
            throw new ReservationAlreadyCancelledException();

        reservation.Status = ReservationStatus.Active;

        reservation.ExpiresAt = utcNow.AddDays(3);

        AddDomainEvent(new ReservationFulfilledEvent(
            ReservationId: reservation.Id,
            UserId: reservation.UserId,
            BookCopyId: Id,
            AvailableUntil: reservation.ExpiresAt));

        return reservation;
    }

    public Reservation CancelReservation(
        Guid reservationId,
        string reason)
    {
        Guard.Against.Null(_reservations);

        var reservation = PendingReservations.SingleOrDefault(x => x.Id == reservationId);

        if (reservation is null)
            throw new EntityNotFoundException(
                entityType: nameof(Reservation),
                searchKey: reservationId.ToString());

        if (reservation.Status == ReservationStatus.Cancelled)
            throw new ReservationAlreadyCancelledException();

        reservation.Status = ReservationStatus.Cancelled;

        AddDomainEvent(new ReservationCancelledEvent(
            ReservationId: reservation.Id,
            UserId: reservation.UserId,
            BookCopyId: Id,
            Reason: reason));

        return reservation;
    }
    
    public void Retire()
    {
        foreach (var activeReservation in PendingReservations.Select(x => x.Id))
        {
            CancelReservation(
                reservationId: activeReservation,
                reason: "BOOK_RETIRED");
        }

        IsRetired = true;
    }
}