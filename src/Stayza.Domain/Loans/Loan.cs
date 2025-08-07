using System.Text.Json.Serialization;

namespace Stayza.Domain.Loans;

public class Loan
{
    [JsonInclude]
    public Guid Id { get; private set; }

    [JsonInclude]
    public Guid BookCopyId { get; private set; }

    [JsonInclude]
    public string UserId { get; private set; }

    [JsonInclude]
    public TimeRange TimeRange { get; set; }

    [JsonInclude]
    public DateTimeOffset? ReturnDate { get; private set; }

    [JsonInclude]
    public bool IsReturned => ReturnDate.HasValue;

    [Obsolete("Used only by ef core")]
    public Loan()
    {
    }
    
    public Loan(
        Guid bookCopyId,
        string userId,
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