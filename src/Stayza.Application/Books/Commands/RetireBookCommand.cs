using FluentValidation;

namespace Stayza.Application.Books.Commands;

public record RetireBookCommand(Guid BookId);

public class RetireBookCommandValidator : AbstractValidator<RetireBookCommand>
{
    public RetireBookCommandValidator()
    {
        RuleFor(x => x.BookId)
            .NotEmpty();
    }
}