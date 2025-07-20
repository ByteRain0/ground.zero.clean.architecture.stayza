namespace Stayza.Domain.BookCopyAggregate;

public class Loan
{
    public Guid Id { get; private set; }

    public Guid BookCopyId { get; private set; }

    public Guid UserId { get; private set; }

    public TimeRange TimeRange { get; set; }

    public DateTimeOffset? ReturnDate { get; private set; }

    public bool IsReturned => ReturnDate.HasValue;

    public Loan(
        Guid bookCopyId,
        Guid userId,
        TimeRange timeRange,
        Guid? id)
    {
        BookCopyId = bookCopyId;
        UserId = userId;
        TimeRange = timeRange;
        ReturnDate = null;
        Id = id ?? Guid.NewGuid();
    }

    public void MarkAsReturned(DateTimeOffset returnedAt)
    {
        if (IsReturned)
            throw new InvalidOperationException("Loan already returned.");

        ReturnDate = returnedAt;
    }
}