using FluentValidation;

namespace Stayza.Application.Books.Commands;

public record MarkBookForRemoval(Guid BookId);

public class MarkBookForRemovalValidator : AbstractValidator<MarkBookForRemoval>
{
    public MarkBookForRemovalValidator()
    {
        RuleFor(x => x.BookId)
            .NotEmpty();
    }
}