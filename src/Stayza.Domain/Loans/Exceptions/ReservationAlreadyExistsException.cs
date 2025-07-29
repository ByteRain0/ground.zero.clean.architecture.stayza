namespace Stayza.Domain.Loans.Exceptions;

public class ReservationAlreadyExistsException : Exception
{
    public string UserId { get; set; }

    public ReservationAlreadyExistsException(string userId)
        : base(message: "User already has an active reservation for this copy.")
    {
        UserId = userId;
    }
}