namespace Stayza.Domain.BookAggregate;

public interface IBooksRepository
{
    Task<Book> Add(Book book);

    Task<Book> Update(Book book);

    Task<Book> GetById(Guid id, CancellationToken cancellationToken);

    Task<Book> GetByIsbn(string isbn, CancellationToken cancellationToken);
}