namespace Stayza.Domain.Loans.Exceptions;

public class BookCopyAlreadyLoanedException : Exception
{
    public string UserId { get; set; }

    public BookCopyAlreadyLoanedException(string userId)
        : base(message: "Copy already loaned to user.")
    {
        UserId = userId;
    }
}