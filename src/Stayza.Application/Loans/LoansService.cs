using Stayza.Application.Loans.Commands;
using Stayza.Application.Loans.Queries;
using Stayza.Core.PagingAndSorting;
using Stayza.Domain.Loans;

namespace Stayza.Application.Loans;

public class LoansService(
    ILoansRepository repository,
    TimeProvider timeProvider)
{
    public async Task<Reservation> ReserveBookCopy(ReserveBookCopyCommand command)
    {
        var bookCopy = await repository.GetBookCopyById(command.BookCopyId, CancellationToken.None);
        var reservation =
            bookCopy.Reserve(userId: command.UserId, utcNow: timeProvider.GetUtcNow());
        await repository.UpdateBookCopy(bookCopy);

        return reservation;
    }

    public async Task<Reservation> CancelReservation(CancelReservationCommand command)
    {
        var bookCopy = await repository.GetBookCopyById(command.BookCopyId, CancellationToken.None);
        var cancelledReservation =
            bookCopy.CancelReservation(reservationId: command.ReservationId, reason: command.Reason);
        await repository.UpdateBookCopy(bookCopy);

        return cancelledReservation;
    }

    public async Task<Reservation> ExpireReservation(ExpireReservationCommand command)
    {
        var bookCopy = await repository.GetBookCopyById(command.BookCopyId, CancellationToken.None);
        var expiredReservation =
            bookCopy.ExpireReservation(reservationId: command.ReservationId, utcNow: timeProvider.GetUtcNow());
        await repository.UpdateBookCopy(bookCopy);

        return expiredReservation;
    }

    public async Task<Reservation> FulfillReservation(FulFillReservationCommand command)
    {
        var bookCopy = await repository.GetBookCopyById(command.BookCopyId, CancellationToken.None);
        var fulfilledReservation =
            bookCopy.FulfillReservation(reservationId: command.ReservationId, utcNow: timeProvider.GetUtcNow());
        await repository.UpdateBookCopy(bookCopy);

        return fulfilledReservation;
    }

    public async Task<Loan> StartLoan(StartLoanCommand command)
    {
        var bookCopy = await repository.GetBookCopyById(command.BookCopyId, CancellationToken.None);
        var loan = bookCopy.StartLoan(userId: command.UserId, timeProvider.GetUtcNow());
        await repository.UpdateBookCopy(bookCopy);

        return loan;
    }

    public async Task<Loan> ReturnBookCopy(ReturnBookCopyCommand command)
    {
        var bookCopy = await repository.GetBookCopyById(command.BookCopyId, CancellationToken.None);
        var loan = bookCopy.Return(command.UserId, timeProvider.GetUtcNow());
        await repository.UpdateBookCopy(bookCopy);

        return loan;
    }

    public Task<Loan> GetLoanById(Guid id, CancellationToken cancellationToken) 
        => repository.GetLoanById(id, cancellationToken);

    public Task<PagedList<Loan>> GetLoans(
        GetLoansQuery query,
        CancellationToken cancellationToken) =>
        repository.GetLoans(
            page: query.Page, 
            pageSize: query.PageSize, 
            sortOrder: query.SortOrder,
            sortColumn: query.SortColumn, 
            userId: query.UserId,
            cancellationToken: cancellationToken);
}