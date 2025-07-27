namespace Stayza.Domain.Loans.Exceptions;

public class ReservationAlreadyCancelledException : Exception
{
    public ReservationAlreadyCancelledException()
        : base(message: "Reservation was already cancelled")
    {
    }
}