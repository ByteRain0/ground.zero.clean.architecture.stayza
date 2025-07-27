using Stayza.Core.PagingAndSorting;

namespace Stayza.Domain.Loans;

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
    Task<List<Loan>> GetLoansThatAreOverdueAfter(
        DateTimeOffset endTimeOffset,
        CancellationToken cancellationToken);

    Task<List<Reservation>> GetReservationThatShouldExpireAfter(
        DateTimeOffset endTimeOffset,
        CancellationToken cancellationToken);

    /// <summary>
    /// Remove reservations based on already set status.
    /// </summary>
    /// <returns></returns>
    Task RemoveExpiredOrCancelledReservations();

    Task<Loan> GetLoanById(Guid id, CancellationToken cancellationToken);

    Task<PagedList<Loan>> GetLoans(
        int page,
        int pageSize,
        string? sortColumn,
        SortOrder? sortOrder,
        Guid? userId,
        CancellationToken cancellationToken);
}