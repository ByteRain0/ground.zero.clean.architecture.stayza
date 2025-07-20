namespace Stayza.Domain.BookCopyAggregate;

public interface ILoansRepository
{
    Task<BookCopy> GetBookCopyById(Guid id, CancellationToken cancellationToken);

    Task<BookCopy> UpdateBookCopy(BookCopy bookCopy);

    /// <summary>
    /// Used as a cron job to cycle through loans that are overdue and notify users about return policies. 
    /// </summary>
    /// <param name="endTimeOffset"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Loan> GetLoansThatAreOverdueAfter(
        DateTimeOffset endTimeOffset,
        CancellationToken cancellationToken);

    Task<Reservation> GetReservationThatShouldExpireAfter(
        DateTimeOffset endTimeOffset,
        CancellationToken cancellationToken);

    /// <summary>
    /// Remove reservations based on already set status.
    /// </summary>
    /// <returns></returns>
    Task RemoveExpiredOrCancelledReservations();
}