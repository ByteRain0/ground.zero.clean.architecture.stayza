using Stayza.Application.Loans.Commands;
using Stayza.Domain.BookCopyAggregate;

namespace Stayza.Application.Loans;

public class LoansService
{
    private readonly ILoansRepository _repository;

    private readonly TimeProvider _timeProvider;

    public LoansService(
        ILoansRepository repository,
        TimeProvider timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public async Task<Reservation> ReserveBookCopy(ReserveBookCopyCommand command)
    {
        var bookCopy = await _repository.GetBookCopyById(command.BookCopyId, CancellationToken.None);
        var reservation =
            bookCopy.Reserve(userId: command.UserId, utcNow: _timeProvider.GetUtcNow());
        await _repository.UpdateBookCopy(bookCopy);

        return reservation;
    }

    public async Task<Reservation> CancelReservation(CancelReservationCommand command)
    {
        var bookCopy = await _repository.GetBookCopyById(command.BookCopyId, CancellationToken.None);
        var cancelledReservation =
            bookCopy.CancelReservation(reservationId: command.ReservationId, reason: command.Reason);
        await _repository.UpdateBookCopy(bookCopy);

        return cancelledReservation;
    }

    public async Task<Reservation> ExpireReservation(ExpireReservationCommand command)
    {
        var bookCopy = await _repository.GetBookCopyById(command.BookCopyId, CancellationToken.None);
        var expiredReservation =
            bookCopy.ExpireReservation(reservationId: command.ReservationId, utcNow: _timeProvider.GetUtcNow());
        await _repository.UpdateBookCopy(bookCopy);

        return expiredReservation;
    }

    public async Task<Reservation> FulfillReservation(FulFillReservationCommand command)
    {
        var bookCopy = await _repository.GetBookCopyById(command.BookCopyId, CancellationToken.None);
        var fulfilledReservation =
            bookCopy.FulfillReservation(reservationId: command.ReservationId, utcNow: _timeProvider.GetUtcNow());
        await _repository.UpdateBookCopy(bookCopy);

        return fulfilledReservation;
    }

    public async Task<Loan> StartLoan(StartLoanCommand command)
    {
        var bookCopy = await _repository.GetBookCopyById(command.BookCopyId, CancellationToken.None);
        var loan = bookCopy.StartLoan(userId: command.UserId, _timeProvider.GetUtcNow());
        await _repository.UpdateBookCopy(bookCopy);

        return loan;
    }

    public async Task<Loan> ReturnBookCopy(ReturnBookCopyCommand command)
    {
        var bookCopy = await _repository.GetBookCopyById(command.BookCopyId, CancellationToken.None);
        var loan = bookCopy.Return(command.UserId, _timeProvider.GetUtcNow());
        await _repository.UpdateBookCopy(bookCopy);

        return loan;
    }
}