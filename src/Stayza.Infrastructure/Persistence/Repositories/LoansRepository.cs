using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stayza.Core.Exceptions;
using Stayza.Core.PagingAndSorting;
using Stayza.Domain.BookCopyAggregate;
using Stayza.Infrastructure.Persistence.Extensions;

namespace Stayza.Infrastructure.Persistence.Repositories;

internal class LoansRepository : ILoansRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    private readonly ILogger<LoansRepository> _logger;

    public LoansRepository(
        ApplicationDbContext applicationDbContext,
        ILogger<LoansRepository> logger)
    {
        _applicationDbContext = applicationDbContext;
        _logger = logger;
    }

    public async Task<BookCopy> GetBookCopyById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var bookCopy = await _applicationDbContext.BookCopies.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (bookCopy is null)
        {
            throw new EntityNotFoundException(
                entityType: nameof(BookCopy),
                searchKey: id.ToString());
        }

        return bookCopy;
    }

    public async Task<BookCopy> UpdateBookCopy(BookCopy bookCopy)
    {
        _applicationDbContext.Update(bookCopy);
        await _applicationDbContext.SaveChangesAsync();

        return bookCopy;
    }

    public async Task<List<Loan>> GetLoansThatAreOverdueAfter(
        DateTimeOffset endTimeOffset,
        CancellationToken cancellationToken) =>
        await _applicationDbContext.Loans
            .Where(x => x.IsReturned == false)
            .Where(x => x.TimeRange.End < endTimeOffset)
            .ToListAsync(cancellationToken: cancellationToken);

    public Task<List<Reservation>> GetReservationThatShouldExpireAfter(
        DateTimeOffset endTimeOffset,
        CancellationToken cancellationToken) =>
        _applicationDbContext.Reservations
            .Where(x => x.Status == ReservationStatus.Active)
            .Where(x => x.ExpiresAt > endTimeOffset)
            .ToListAsync(cancellationToken: cancellationToken);

    public async Task RemoveExpiredOrCancelledReservations()
    {
        var countOfEntries = await _applicationDbContext.Reservations.Where(x =>
                x.Status == ReservationStatus.Expired
                || x.Status == ReservationStatus.Cancelled)
            .ExecuteDeleteAsync();
        
        // An example of a human focused log entry that can bring value.
        _logger.LogInformation("A total of {deletedReservationsCount} expired and/or cancelled reservations have been deleted.", countOfEntries);
    }

    public async Task<Loan> GetLoanById(Guid id, CancellationToken cancellationToken)
    {
        var loan = await _applicationDbContext.Loans
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);

        if (loan is null)
        {
            throw new EntityNotFoundException(
                entityType: nameof(Loan),
                searchKey: id.ToString());
        }

        return loan;
    }

    public async Task<PagedList<Loan>> GetLoans(
        int page, 
        int pageSize, 
        string? sortColumn, 
        SortOrder? sortOrder,
        Guid? userId,
        CancellationToken cancellationToken)
    {
        var dbQuery = _applicationDbContext.Loans
            .Where(x => !userId.HasValue || x.UserId == userId.Value)
            .AsQueryable();
        
        if (sortOrder is not null)
        {
            if (sortOrder == SortOrder.Ascending)
            {
                dbQuery.OrderBy(GetLoanSortColumn(sortColumn));
            }
            else
            {
                dbQuery.OrderByDescending(GetLoanSortColumn(sortColumn));
            }
        }
        
        
        return await PagedListExtensions<Loan>.CreateAsync(
            source: dbQuery,
            page: page,
            pageSize: pageSize,
            cancellationToken: cancellationToken);
    }

    private static Expression<Func<Loan, object>> GetLoanSortColumn(string sortColumn)
    {
        return sortColumn switch
        {
            "isReturned" => loan => loan.IsReturned,
            "start" => loan => loan.TimeRange.Start,
            _ => loan => loan.TimeRange.End
        };
    }
}