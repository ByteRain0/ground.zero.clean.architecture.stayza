namespace Stayza.Domain.BookCopyAggregate.Exceptions;

public class BookCopyAlreadyLoanedException : Exception
{
    public Guid UserId { get; set; }

    public BookCopyAlreadyLoanedException(Guid userId)
        : base(message: "Copy already loaned to user.")
    {
        UserId = userId;
    }
}