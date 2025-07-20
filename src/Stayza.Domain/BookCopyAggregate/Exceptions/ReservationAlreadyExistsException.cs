namespace Stayza.Domain.BookCopyAggregate.Exceptions;

public class ReservationAlreadyExistsException : Exception
{
    public Guid UserId { get; set; }

    public ReservationAlreadyExistsException(Guid userId)
        : base(message: "User already has an active reservation for this copy.")
    {
        UserId = userId;
    }
}