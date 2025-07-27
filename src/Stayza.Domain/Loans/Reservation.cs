using Stayza.Core.Entity;

namespace Stayza.Domain.Loans;

public class Reservation : Entity
{
    public Guid UserId { get; private set; }

    public Guid BookCopyId { get; private set; }

    public DateTimeOffset ReservedAt { get; private set; }

    /// <summary>
    /// Example if you don't like using ValueObjects :P
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }

    public ReservationStatus Status { get; set; }

    [Obsolete("Used only by ef core")]
    public Reservation()
    {
    }
    
    public Reservation(
        Guid userId,
        DateTimeOffset reservedAt,
        Guid bookCopyId,
        Guid id) : base(id)
    {
        UserId = userId;
        ReservedAt = reservedAt;
        BookCopyId = bookCopyId;
        Status = ReservationStatus.Active;
    }
}