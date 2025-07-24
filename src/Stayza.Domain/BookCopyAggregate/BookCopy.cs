using Stayza.Core.Entity;
using Stayza.Core.Exceptions;
using Stayza.Domain.BookCopyAggregate.Events;
using Stayza.Domain.BookCopyAggregate.Exceptions;

namespace Stayza.Domain.BookCopyAggregate;

public class BookCopy : AggregateRoot
{
    public Guid BookId { get; set; }

    public bool IsRetired { get; set; }
        = false;

    private Loan? _currentLoan;

    public bool IsAvailable => _currentLoan == null || _currentLoan.IsReturned;

    public bool IsLoaned => _currentLoan != null && !_currentLoan.IsReturned;

    /// <summary>
    /// TODO: At some point the amount of reservations will increase so we need a way to remove/archive old reservations.
    /// - Verificare ca o carte nu poate fi rezervată de două ori de același utilizator pe același BookCopy
    /// - Posibilă validare să nu poți rezerva o carte de mai multe ori consecutiv dacă ai deja o rezervare neexpirată
    /// - Poți limita numărul maxim de rezervări active per BookCopy
    /// </summary>
    private readonly List<Reservation> _reservations = new();

    public IReadOnlyCollection<Reservation> ActiveReservations =>
        _reservations.Where(r => r.Status == ReservationStatus.Active).ToList();

    [Obsolete("Used only by ef core")]
    public BookCopy()
    {
    }
    
    public BookCopy(Guid id, Guid bookId)
        : base(id)
    {
        BookId = bookId;
    }

    public Reservation Reserve(
        Guid userId,
        DateTimeOffset utcNow)
    {
        if (!IsAvailable && !IsRetired)
            throw new BookNotAvailableForReservation();

        if (_reservations.Any(r => r.UserId == userId && r.Status == ReservationStatus.Active))
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
        Guid userId,
        DateTimeOffset utcNow)
    {
        if (IsLoaned)
            throw new BookCopyAlreadyLoanedException(userId);

        var reservation = _reservations.SingleOrDefault(r =>
            r.UserId == userId &&
            r.Status == ReservationStatus.Active);

        if (reservation is null)
            throw new EntityNotFoundException(
                entityType: nameof(BookCopy),
                searchKey: $"userId: {userId}");

        _reservations.Remove(reservation);
        
        _currentLoan = new Loan(
            bookCopyId: Id,
            userId: userId,
            timeRange: new TimeRange(
                start: utcNow,
                end: utcNow.AddDays(14)),
            id: Guid.NewGuid());

        AddDomainEvent(new BookLoanedEvent(
            BookCopyId: Id,
            UserId: userId,
            LoanId: _currentLoan.Id,
            LoanDate: _currentLoan.TimeRange.Start,
            DueDate: _currentLoan.TimeRange.End));

        return _currentLoan;
    }

    public Loan Return(
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

        return _currentLoan;
    }

    public Reservation ExpireReservation(
        Guid reservationId,
        DateTimeOffset utcNow)
    {
        var reservation = _reservations.SingleOrDefault(x => x.Id == reservationId);

        if (reservation is null)
            throw new EntityNotFoundException(
                entityType: nameof(Reservation),
                searchKey: reservationId.ToString());

        if (utcNow < reservation.ExpiresAt)
            throw new InvalidOperationException("Cannot expire still-valid reservation");

        reservation.Status = ReservationStatus.Expired;

        AddDomainEvent(new ReservationExpiredEvent(
            ReservationId: reservation.Id,
            UserId: reservation.UserId,
            BookCopyId: Id));

        return reservation;
    }

    public Reservation FulfillReservation(
        Guid reservationId,
        DateTimeOffset utcNow)
    {
        var reservation = _reservations.SingleOrDefault(x => x.Id == reservationId);

        if (reservation is null)
            throw new EntityNotFoundException(
                entityType: nameof(Reservation),
                searchKey: reservationId.ToString());

        if (reservation.Status == ReservationStatus.Cancelled)
            throw new ReservationAlreadyCancelledException();

        reservation.Status = ReservationStatus.Fulfilled;

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
        var reservation = ActiveReservations.SingleOrDefault(x => x.Id == reservationId);

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
}