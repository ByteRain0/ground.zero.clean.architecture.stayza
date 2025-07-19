namespace Stayza.Domain.BookCopyAggregate;

/// <summary>
/// Internal entity to BookCopy aggregate
/// </summary>
public class Reservation
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    
    public Guid BookCopyId { get; set; }
    public DateTimeOffset ReservedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }

    public ReservationStatus Status { get; set; }
    
    public Reservation(
        Guid userId,
        DateTimeOffset reservedAt,
        Guid bookCopyId,
        Guid? id)
    {
        UserId = userId;
        ReservedAt = reservedAt;
        BookCopyId = bookCopyId;
        Status = ReservationStatus.Active;
        Id = id ?? Guid.NewGuid();
    }
    
    public bool IsExpired(DateTimeOffset utcNow) => utcNow > ExpiresAt;
    
    public void FulFill(DateTimeOffset utcNow)
    {
        Status = ReservationStatus.Fulfilled;
        ExpiresAt = utcNow.AddDays(3);
        // Probably here would be an ideal place for a one-time job in the future to check expired reservations
        // Or do the easy and do a recurring job :P
    }

    public void Expire()
    {
        Status = ReservationStatus.Expired;
        // TODO: sent a notification about the expiration.
    }

    public void CancelReservation()
    {
        Status = ReservationStatus.Cancelled;
        // TODO: sent a notification about cancellation so next reservation can be picked.
    }
}