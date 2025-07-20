using Stayza.Domain.BookAggregate;

namespace Stayza.Application.BookService;

public class BookService
{
    public async Task<Book> AddBook()
    {
        return default;
    }
    
    public async Task<Book> GetBookById()
    {
        return default;
    }

    public async Task MarkForDeletion()
    {
        // Cannot remove until copies are reserved / loaned
        // Cancel all existing reservations and add reason.
        // Mark copy for deletion so no new reservations can be made.
        return;
    }

    public async Task DeleteBookCopy(Guid bookCopyId)
    {
        return;
    }

    public async Task AddBookCopy()
    {
        return;
    }

    public async Task RemoveBookCopy()
    {
        return;
    }
}