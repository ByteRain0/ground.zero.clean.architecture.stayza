using FluentValidation;

namespace Stayza.Application.Loans.Commands;

public record StartLoanCommand(
    Guid BookCopyId,
    string UserId);


public class StartLoanCommandValidator : AbstractValidator<ReturnBookCopyCommand>
{
    public StartLoanCommandValidator()
    {
        RuleFor(x => x.BookCopyId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}