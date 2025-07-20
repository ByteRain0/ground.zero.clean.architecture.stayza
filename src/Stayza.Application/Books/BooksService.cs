using Stayza.Application.Books.Commands;
using Stayza.Domain.BookAggregate;
using Stayza.Domain.BookCopyAggregate;

namespace Stayza.Application.Books;

public class BooksService
{
    private readonly IBookRepository _repository;

    public BooksService(IBookRepository repository)
    {
        _repository = repository;
    }

    public Task<Book> AddBook(AddBookCommand command)
    {
        return _repository.Add(new Book(
            title: command.Title,
            author: command.Author,
            isbn: command.ISBN,
            id: Guid.NewGuid()));
    }

    public Task<Book> GetBookById(Guid id, CancellationToken cancellationToken)
    {
        return _repository.GetById(id, cancellationToken);
    }
    
    public Task<Book> GetBookByIsbn(string isbn, CancellationToken cancellationToken)
    {
        return _repository.GetByIsbn(isbn, cancellationToken);
    }

    public async Task<Book> MarkForRemoval(MarkBookForRemoval command)
    {
        var book = await _repository.GetById(command.BookId, CancellationToken.None);

        foreach (var bookCopy in book.Copies)
        {
            foreach (var activeReservation in bookCopy.ActiveReservations)
            {
                bookCopy.CancelReservation(
                    reservation: activeReservation,
                    reason: "Book marked for deletion");
            }

            bookCopy.IsMarkedForRemoval = true;
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