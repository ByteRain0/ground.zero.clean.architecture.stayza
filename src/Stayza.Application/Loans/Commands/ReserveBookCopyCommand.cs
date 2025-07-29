using FluentValidation;

namespace Stayza.Application.Loans.Commands;

public record ReserveBookCopyCommand(
    Guid BookCopyId,
    string UserId);

public class ReserveBookCopyCommandValidator : AbstractValidator<ReserveBookCopyCommand>
{
    public ReserveBookCopyCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.BookCopyId)
            .NotEmpty();
    }
}