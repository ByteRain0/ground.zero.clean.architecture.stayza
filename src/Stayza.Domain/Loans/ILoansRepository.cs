using Stayza.Core.PagingAndSorting;

namespace Stayza.Domain.Loans;

public interface ILoansRepository
{
    Task<BookCopy> GetBookCopyById(Guid id, CancellationToken cancellationToken);

    Task<BookCopy> UpdateBookCopy(BookCopy bookCopy);

    Task<List<Loan>> GetLoansThatAreOverdueAfter(
        DateTimeOffset endTimeOffset,
        CancellationToken cancellationToken);

    Task<List<Reservation>> GetReservationThatShouldExpire(
        DateTimeOffset after,
        CancellationToken cancellationToken);

    Task RemoveExpiredAndCancelledReservations(DateTimeOffset after);

    Task<List<Reservation>> GetExpiredReservations(DateTimeOffset after);
    
    Task RemoveCancelledReservation();

    Task<Loan> GetLoanById(Guid id, CancellationToken cancellationToken);

    Task<PagedList<Loan>> GetLoans(
        int page,
        int pageSize,
        string? sortColumn,
        SortOrder? sortOrder,
        string? userId,
        CancellationToken cancellationToken);
}