using FluentValidation;

namespace Stayza.Application.Loans.Commands;

public record CancelReservationCommand(
    Guid BookCopyId,
    Guid ReservationId,
    string Reason);


public class CancelReservationCommandValidator : AbstractValidator<CancelReservationCommand>
{
    public CancelReservationCommandValidator()
    {
        RuleFor(x => x.BookCopyId)
            .NotEmpty();

        RuleFor(x => x.ReservationId)
            .NotEmpty();

        RuleFor(x => x.Reason)
            .NotEmpty();
    }
}