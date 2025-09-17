using FluentValidation;

namespace Stayza.Application.Books.Commands;

public record AddBookCommand(
    string Title,
    string Author,
    string ISBN);


public class AddBookValidator : AbstractValidator<AddBookCommand>
{
    public AddBookValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty();

        RuleFor(x => x.Author)
            .NotEmpty();

        RuleFor(x => x.ISBN)
            .NotEmpty();
    }
}
