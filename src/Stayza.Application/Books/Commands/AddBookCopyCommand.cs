using FluentValidation;

namespace Stayza.Application.Books.Commands;

public record AddBookCopyCommand(Guid BookId);

public class AddBookCopyCommandValidator : AbstractValidator<AddBookCopyCommand>
{
    public AddBookCopyCommandValidator()
    {
        RuleFor(x => x.BookId)
            .NotEmpty();
    }
}