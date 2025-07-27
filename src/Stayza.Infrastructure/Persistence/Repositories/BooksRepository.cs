using Microsoft.EntityFrameworkCore;
using Stayza.Core.Exceptions;
using Stayza.Domain.Books;

namespace Stayza.Infrastructure.Persistence.Repositories;

public class BooksRepository : IBooksRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    public BooksRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<Book> Add(Book book)
    {
        await _applicationDbContext.Books.AddAsync(book);
        await _applicationDbContext.SaveChangesAsync();

        return book;
    }

    public async Task<Book> Update(Book book)
    {
        _applicationDbContext.Books.Update(book);
        await _applicationDbContext.SaveChangesAsync();
        
        return book;
    }

    public async Task<Book> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var book = await _applicationDbContext.Books
            .Include(x => x.Copies)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (book is null)
        {
            throw new EntityNotFoundException(
                entityType: nameof(Book),
                searchKey: id.ToString());
        }

        return book;
    }

    public async Task<Book> GetByIsbn(
        string isbn,
        CancellationToken cancellationToken)
    {
        var book = await _applicationDbContext.Books
            .Include(x => x.Copies)
            .FirstOrDefaultAsync(x => x.ISBN == isbn, cancellationToken);

        if (book is null)
        {
            throw new EntityNotFoundException(
                entityType: nameof(Book),
                searchKey: isbn);
        }

        return book;
    }
}