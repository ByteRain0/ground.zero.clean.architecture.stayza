using Stayza.Application.Books.Commands;
using Stayza.Domain.Books;
using Stayza.Domain.Loans;

namespace Stayza.Application.Books;

public class BooksService(IBooksRepository repository)
{
    public Task<Book> AddBook(AddBookCommand command) =>
        repository.Add(new Book(
            title: command.Title,
            author: command.Author,
            isbn: command.ISBN,
            id: Guid.NewGuid()));

    public Task<Book> GetBookById(Guid id, CancellationToken cancellationToken) => repository.GetById(id, cancellationToken);

    public Task<Book> GetBookByIsbn(string isbn, CancellationToken cancellationToken) => repository.GetByIsbn(isbn, cancellationToken);

    public async Task<Book> Retire(RetireBookCommand command)
    {
        var book = await repository.GetById(command.BookId, CancellationToken.None);

        foreach (var bookCopy in book.Copies)
        {
            bookCopy.Retire();
        }

        await repository.Update(book);

        return book;
    }

    public async Task<Book> RemoveBookCopy(RemoveBookCopyCommand command)
    {
        var book = await repository.GetById(command.BookId, CancellationToken.None);
        book.RemoveCopy(command.BookCopyId);
        await repository.Update(book);

        return book;
    }

    public async Task<BookCopy> AddBookCopy(AddBookCopyCommand command)
    {
        var book = await repository.GetById(command.BookId, CancellationToken.None);
        var bookCopy = book.AddCopy(Guid.NewGuid());
        await repository.Update(book);

        return bookCopy;
    }
}