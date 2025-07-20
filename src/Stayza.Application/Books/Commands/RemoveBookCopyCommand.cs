using FluentValidation;

namespace Stayza.Application.Books.Commands;

public record RemoveBookCopyCommand(Guid BookId, Guid BookCopyId);

public class RemoveBookCopyCommandValidator : AbstractValidator<RemoveBookCopyCommand>
{
    public RemoveBookCopyCommandValidator()
    {
        RuleFor(x => x.BookId)
            .NotEmpty();

        RuleFor(x => x.BookCopyId)
            .NotEmpty();
    }
}