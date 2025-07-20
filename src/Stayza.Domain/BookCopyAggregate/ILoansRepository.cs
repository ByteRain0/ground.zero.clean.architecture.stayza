namespace Stayza.Domain.BookCopyAggregate;

public interface ILoansRepository
{
    Task<BookCopy> GetBookCopyById(Guid id, CancellationToken cancellationToken);

    Task<BookCopy> UpdateBookCopy(BookCopy bookCopy);
}