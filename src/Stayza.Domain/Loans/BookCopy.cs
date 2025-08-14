using System.Text.Json.Serialization;
using Ardalis.GuardClauses;
using Stayza.Core.Entity;
using Stayza.Core.Exceptions;
using Stayza.Core.Telemetry;
using Stayza.Domain.Loans.Events;
using Stayza.Domain.Loans.Exceptions;

namespace Stayza.Domain.Loans;

public class BookCopy : AggregateRoot
{
    [JsonInclude]
    public Guid BookId { get; set; }

    [JsonInclude]
    public bool IsRetired { get; private set; } = false;

    [JsonInclude]
    public Loan? CurrentLoan { get; private set; }

    [JsonInclude]
    public bool IsAvailable => CurrentLoan == null || CurrentLoan.IsReturned;

    [JsonInclude]
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

    //TODO: HW - based on this example set up the rest of the methods with telemetry data.
    public Reservation Reserve(
        string userId,
        DateTimeOffset utcNow)
    {
        using var reserveActivity = RunTimeDiagnosticConfig.Source.StartActivity("Reserve book copy");

        reserveActivity?
            .SetBookCopyId(Id)
            .SetUserId(userId)
            .SetDateTimeOffset(utcNow);

        Guard.Against.Null(_reservations);

        if (IsRetired)
        {
            reserveActivity?.AddTag("isRetired", true);
            var exception = new BookNotAvailableForReservation();
            reserveActivity.AddExceptionAndFail(exception);
            throw exception;
        }

        var existingReservation = _reservations.FirstOrDefault(r =>
            r.UserId == userId && r.Status == ReservationStatus.Pending);

        if (existingReservation is not null)
        {
            reserveActivity.SetReservationId(existingReservation.Id)
                ?.SetTag("reservedAt", existingReservation.ReservedAt)
                ?.SetTag("expiresAt", existingReservation.ExpiresAt);

            var exception = new EntityAlreadyExistsException(
                message: $"Reservation already exists. User: {userId}, ReservationId: {existingReservation.Id}");

            reserveActivity.AddExceptionAndFail(exception);
            throw exception;
        }

        var reservation = new Reservation(
            userId: userId,
            reservedAt: utcNow,
            bookCopyId: Id,
            id: Guid.NewGuid());

        _reservations.Add(reservation);

        return reservation;
    }

    // HW: add OTel activities to better understand how the app behaves.
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
        // Here should be the logic placed to fufill the next pending reservation
        // pending -> not cancelled not expired
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