using FluentValidation;

namespace Stayza.Application.Loans.Commands;

public record ReturnBookCopyCommand(
    Guid BookCopyId,
    Guid UserId);


public class ReturnBookCopyCommandValidator : AbstractValidator<ReturnBookCopyCommand>
{
    public ReturnBookCopyCommandValidator()
    {
        RuleFor(x => x.BookCopyId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}