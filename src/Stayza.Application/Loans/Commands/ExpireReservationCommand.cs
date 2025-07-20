using FluentValidation;

namespace Stayza.Application.Loans.Commands;

public record ExpireReservationCommand(
    Guid BookCopyId,
    Guid ReservationId);


public class ExpireReservationCommandValidator : AbstractValidator<ExpireReservationCommand>
{
    public ExpireReservationCommandValidator()
    {
        RuleFor(x => x.BookCopyId)
            .NotEmpty();

        RuleFor(x => x.ReservationId)
            .NotEmpty();
    }
}