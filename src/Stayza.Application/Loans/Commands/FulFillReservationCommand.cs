using FluentValidation;

namespace Stayza.Application.Loans.Commands;

public record FulFillReservationCommand(
    Guid BookCopyId, 
    Guid ReservationId);


public class FulFillReservationCommandValidator : AbstractValidator<FulFillReservationCommand>
{
    public FulFillReservationCommandValidator()
    {
        RuleFor(x => x.BookCopyId)
            .NotEmpty();

        RuleFor(x => x.ReservationId)
            .NotEmpty();
    }
}