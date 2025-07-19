using Stayza.Core.Entity;

namespace Stayza.Domain.BookCopyAggregate;

public class Reservation : Entity
{
    public Guid UserId { get; private set; }
    
    public Guid BookCopyId { get; private set; }
    
    public DateTimeOffset ReservedAt { get; private set; }
    
    public DateTimeOffset ExpiresAt { get; set; }

    public ReservationStatus Status { get; set; }
    
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