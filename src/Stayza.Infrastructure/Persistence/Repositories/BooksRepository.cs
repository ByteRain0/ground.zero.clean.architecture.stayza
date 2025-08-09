using Microsoft.EntityFrameworkCore;
using Stayza.Core.Exceptions;
using Stayza.Domain.Books;

namespace Stayza.Infrastructure.Persistence.Repositories;

public class BooksRepository(ApplicationDbContext applicationDbContext) : IBooksRepository
{
    public async Task<Book> Add(Book book)
    {
        if (await applicationDbContext.Books.AnyAsync(x => x.ISBN == book.ISBN))
        {
            throw new EntityAlreadyExistsException($"Book with ISBN {book.ISBN} already exists.");
        }
        
        await applicationDbContext.Books.AddAsync(book);
        await applicationDbContext.SaveChangesAsync();

        return book;
    }

    public async Task<Book> Update(Book book)
    {
        applicationDbContext.Books.Update(book);
        await applicationDbContext.SaveChangesAsync();
        
        return book;
    }

    public async Task<Book> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var book = await applicationDbContext.Books
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
        var book = await applicationDbContext.Books
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