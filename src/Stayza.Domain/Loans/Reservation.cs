using Stayza.Core.Entity;

namespace Stayza.Domain.Loans;

public class Reservation : Entity
{
    public string UserId { get; private set; }

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
        string userId,
        DateTimeOffset reservedAt,
        Guid bookCopyId,
        Guid id) : base(id)
    {
        UserId = userId;
        ReservedAt = reservedAt;
        // Invariant: By default every reservation is maintained for 30 days. Left as magic number on purpose.
        // Invariant: Once fulfilled reservation expiry is reset to +3 days.
        ExpiresAt = reservedAt.AddDays(30);
        BookCopyId = bookCopyId;
        Status = ReservationStatus.Pending;
    }
}