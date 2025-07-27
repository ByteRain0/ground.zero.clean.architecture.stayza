using Stayza.Application.Books.Commands;
using Stayza.Domain.Books;
using Stayza.Domain.Loans;

namespace Stayza.Application.Books;

public class BooksService
{
    private readonly IBooksRepository _repository;

    public BooksService(IBooksRepository repository)
    {
        _repository = repository;
    }

    public Task<Book> AddBook(AddBookCommand command) =>
        _repository.Add(new Book(
            title: command.Title,
            author: command.Author,
            isbn: command.ISBN,
            id: Guid.NewGuid()));

    public Task<Book> GetBookById(Guid id, CancellationToken cancellationToken) => _repository.GetById(id, cancellationToken);

    public Task<Book> GetBookByIsbn(string isbn, CancellationToken cancellationToken) => _repository.GetByIsbn(isbn, cancellationToken);

    public async Task<Book> Retire(RetireBookCommand command)
    {
        var book = await _repository.GetById(command.BookId, CancellationToken.None);

        foreach (var bookCopy in book.Copies)
        {
            foreach (var activeReservation in bookCopy.ActiveReservations.Select(x => x.Id))
            {
                bookCopy.CancelReservation(
                    reservationId: activeReservation,
                    reason: "Book retired.");
            }

            bookCopy.IsRetired = true;
        }

        await _repository.Update(book);

        return book;
    }

    public async Task<Book> RemoveBookCopy(RemoveBookCopyCommand command)
    {
        var book = await _repository.GetById(command.BookId, CancellationToken.None);
        book.RemoveCopy(command.BookCopyId);
        await _repository.Update(book);

        return book;
    }

    public async Task<BookCopy> AddBookCopy(AddBookCopyCommand command)
    {
        var book = await _repository.GetById(command.BookId, CancellationToken.None);
        var bookCopy = book.AddCopy(Guid.NewGuid());
        await _repository.Update(book);

        return bookCopy;
    }
}