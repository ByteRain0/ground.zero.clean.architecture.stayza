namespace Stayza.Domain.Loans.Exceptions;

public class BookNotAvailableForReservation : Exception
{
    public BookNotAvailableForReservation()
        : base(message: "Book is available for loaning. Reservation not required.")
    {
    }
}