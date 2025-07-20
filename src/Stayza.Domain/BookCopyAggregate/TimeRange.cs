using Stayza.Core.Entity;

namespace Stayza.Domain.BookCopyAggregate;

public class TimeRange : ValueObject
{
    public DateTimeOffset Start { get; set; }

    public DateTimeOffset End { get; set; }

    public TimeRange(DateTimeOffset start, DateTimeOffset end)
    {
        if (start > end)
            throw new ArgumentException("Start should not come later than end");

        Start = start;
        End = end;
    }

    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Start;
        yield return End;
    }
}