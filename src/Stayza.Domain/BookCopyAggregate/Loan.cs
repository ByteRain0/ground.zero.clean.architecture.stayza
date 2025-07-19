namespace Stayza.Domain.BookCopyAggregate;

public class Loan
{
    public Guid Id { get; private set; }
    
    public Guid CopyId { get; private set; }
    
    public Guid UserId { get; private set; }
    
    public DateTimeOffset LoanDate { get; private set; }
    
    public DateTimeOffset DueDate { get; private set; }
    
    public DateTimeOffset? ReturnDate { get; private set; }

    public bool IsReturned => ReturnDate.HasValue;

    public Loan(
        Guid copyId,
        Guid userId,
        DateTimeOffset loanDate,
        DateTimeOffset dueDate,
        Guid? id)
    {
        CopyId = copyId;
        UserId = userId;
        LoanDate = loanDate;
        DueDate = dueDate;
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