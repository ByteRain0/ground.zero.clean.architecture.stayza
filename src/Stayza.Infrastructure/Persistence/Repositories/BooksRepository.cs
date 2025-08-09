using Microsoft.EntityFrameworkCore;
using Stayza.Core.Exceptions;
using Stayza.Core.Telemetry;
using Stayza.Domain.Books;

namespace Stayza.Infrastructure.Persistence.Repositories;

public class BooksRepository(ApplicationDbContext applicationDbContext) : IBooksRepository
{
    public async Task<Book> Add(Book book)
    {
        using var dbActivity = RunTimeDiagnosticConfig.Source.StartActivity();
        dbActivity?
            .SetBookIsbn(book.ISBN)
            .SetBookTitle(book.Title);

        if (await applicationDbContext.Books.AnyAsync(x => x.ISBN == book.ISBN))
        {
            var exception = new EntityAlreadyExistsException($"Book with ISBN {book.ISBN} already exists.");
            dbActivity?.AddExceptionAndFail(exception);
            throw exception;
        }

        await applicationDbContext.Books.AddAsync(book);
        await applicationDbContext.SaveChangesAsync();

        return book;
    }

    public async Task<Book> Update(Book book)
    {
        using var dbActivity = RunTimeDiagnosticConfig.Source.StartActivity();
        dbActivity?
            .SetBookId(book.Id)
            .SetBookIsbn(book.ISBN)
            .SetBookTitle(book.Title);

        applicationDbContext.Books.Update(book);
        await applicationDbContext.SaveChangesAsync();

        return book;
    }

    public async Task<Book> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        using var dbActivity = RunTimeDiagnosticConfig.Source.StartActivity();
        dbActivity?
            .SetBookId(id);

        var book = await applicationDbContext.Books
            .Include(x => x.Copies)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (book is null)
        {
            var exception = new EntityNotFoundException(
                entityType: nameof(Book),
                searchKey: id.ToString());

            dbActivity?.AddExceptionAndFail(exception);
            throw exception;
        }

        return book;
    }

    public async Task<Book> GetByIsbn(
        string isbn,
        CancellationToken cancellationToken)
    {
        using var dbActivity = RunTimeDiagnosticConfig.Source.StartActivity();
        dbActivity?
            .SetBookIsbn(isbn);

        var book = await applicationDbContext.Books
            .Include(x => x.Copies)
            .FirstOrDefaultAsync(x => x.ISBN == isbn, cancellationToken);

        if (book is null)
        {
            var exception = new EntityNotFoundException(
                entityType: nameof(Book),
                searchKey: isbn);
            
            dbActivity?.AddExceptionAndFail(exception);
            throw exception;
        }

        return book;
    }
}