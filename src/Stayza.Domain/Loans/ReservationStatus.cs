namespace Stayza.Domain.Loans;

public enum ReservationStatus
{
    Pending, // Reservation is in a queue
    Active, // Reservation is active and book copy can be loaned by the user
    Cancelled
}