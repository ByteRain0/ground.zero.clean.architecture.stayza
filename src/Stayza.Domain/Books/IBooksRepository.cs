namespace Stayza.Domain.Books;

public interface IBooksRepository
{
    Task<Book> AddBook(Book book);

    Task<Book> UpdateBook(Book book);

    Task<Book> GetBookById(Guid id, CancellationToken cancellationToken);

    Task<Book> GetBookByIsbn(string isbn, CancellationToken cancellationToken);
}