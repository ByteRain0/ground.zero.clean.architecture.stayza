using Stayza.Domain.BookAggregate;

namespace Stayza.Infrastructure.Persistence.Repositories;

public class BooksRepository : IBooksRepository
{
    public Task<Book> Add(Book book)
    {
        throw new NotImplementedException();
    }

    public Task<Book> Update(Book book)
    {
        throw new NotImplementedException();
    }

    public Task<Book> GetById(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Book> GetByIsbn(string isbn, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}