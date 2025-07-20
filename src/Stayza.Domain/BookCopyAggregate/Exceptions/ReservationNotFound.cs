namespace Stayza.Domain.BookCopyAggregate.Exceptions;

public class ReservationNotFound : Exception
{
    public ReservationNotFound()
        : base(message: "No active reservation found for this user.")
    {
    }
}