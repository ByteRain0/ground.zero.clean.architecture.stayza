using FluentValidation;

namespace Stayza.Application.Books.Commands;

public class AddBookCommand
{
    public string Title { get; set; }

    public string Author { get; set; }

    public string ISBN { get; set; }
    
    public AddBookCommand()
    {
        
    }

    public AddBookCommand(
        string Title,
        string Author,
        string ISBN)
    {
        this.Author = Author;
        this.Title = Title;
        this.ISBN = ISBN;
    }
}

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
