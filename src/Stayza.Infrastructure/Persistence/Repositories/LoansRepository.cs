using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stayza.Core.Exceptions;
using Stayza.Core.PagingAndSorting;
using Stayza.Core.Telemetry;
using Stayza.Domain.Loans;
using Stayza.Infrastructure.Persistence.Extensions;

namespace Stayza.Infrastructure.Persistence.Repositories;

internal class LoansRepository(
    ApplicationDbContext applicationDbContext,
    ILogger<LoansRepository> logger) : ILoansRepository
{
    public async Task<BookCopy> GetBookCopyById(
        Guid id,
        CancellationToken cancellationToken)
    {
        using var dbActivity = RunTimeDiagnosticConfig.Source.StartActivity();
        dbActivity?
            .SetBookCopyId(id);
        
        var bookCopy = await applicationDbContext.BookCopies
            .Include(x => x.Reservations)
            .Include(x => x.CurrentLoan)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (bookCopy is null)
        {
            var exception = new EntityNotFoundException(
                entityType: nameof(BookCopy),
                searchKey: id.ToString());
            
            dbActivity?.AddExceptionAndFail(exception);
            throw exception;
        }

        return bookCopy;
    }

    public async Task<BookCopy> UpdateBookCopy(BookCopy bookCopy)
    {
        using var dbActivity = RunTimeDiagnosticConfig.Source.StartActivity();
        dbActivity?
            .SetBookCopyId(bookCopy.Id);
        
        applicationDbContext.Update(bookCopy);
        await applicationDbContext.SaveChangesAsync();
        return bookCopy;
    }
    
    public Task<List<Reservation>> GetReservationThatShouldExpire(
        DateTimeOffset after,
        CancellationToken cancellationToken)
    {        
        using var dbActivity = RunTimeDiagnosticConfig.Source.StartActivity();
        dbActivity?.SetTag("afterOffset", after);
        
        return applicationDbContext.Reservations
            .Where(x => x.Status == ReservationStatus.Pending)
            .Where(x => x.ExpiresAt > after)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public Task<List<Reservation>> GetExpiredReservations(DateTimeOffset after) =>
        applicationDbContext
            .Reservations
            .Where(x => x.Status == ReservationStatus.Pending)
            .Where(x => x.ExpiresAt < after)
            .ToListAsync();

    public async Task RemoveCancelledReservation()
    {
        using var dbActivity = RunTimeDiagnosticConfig.Source.StartActivity();
        
        var countOfAffectedEntries = await applicationDbContext
            .Reservations
            .Where(x => x.Status == ReservationStatus.Cancelled)
            .ExecuteDeleteAsync();

        dbActivity?.SetTag("affectedEntriesCount", countOfAffectedEntries);
        
        // An example of a human focused log entry that can bring value.
        logger.LogInformation("A total of {deletedReservationsCount} cancelled have been deleted.",
            countOfAffectedEntries);
    }

    public async Task<Loan> GetLoanById(Guid id, CancellationToken cancellationToken)
    {
        using var dbActivity = RunTimeDiagnosticConfig.Source.StartActivity();
        dbActivity?.SetLoanId(id);
        
        var loan = await applicationDbContext.Loans
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);

        if (loan is null)
        {
            var exception = new EntityNotFoundException(
                entityType: nameof(Loan),
                searchKey: id.ToString());
            
            dbActivity?.AddExceptionAndFail(exception);
            throw exception;
        }

        return loan;
    }

    public async Task<PagedList<Loan>> GetLoans(
        int page,
        int pageSize,
        string? sortColumn,
        SortOrder? sortOrder,
        string? userId,
        CancellationToken cancellationToken)
    {
        var dbQuery = applicationDbContext.Loans
            .Where(x => string.IsNullOrEmpty(userId) || x.UserId == userId)
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