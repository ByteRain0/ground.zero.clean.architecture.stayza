using System.Diagnostics;

namespace Stayza.Core.Telemetry;

public static class TelemetryExtensions
{
    public static Activity? AddExceptionAndFail(this Activity? activity, Exception exception)
    {
        activity?.AddException(exception);
        activity?.SetStatus(ActivityStatusCode.Error);
        return activity;
    }
    
    public static Activity? SetUserId(this Activity? activity, string id)
    {
        activity?.SetTag(GlobalOTelTags.UserId, id);
        return activity;
    }
    
    public static Activity? SetReservationId(this Activity? activity, Guid id)
    {
        activity?.SetTag(GlobalOTelTags.ReservationId, id);
        return activity;
    }
    
    public static Activity? SetBookCopyId(this Activity? activity, Guid id)
    {
        activity?.SetTag(GlobalOTelTags.BookCopyId, id);
        return activity;
    }
    
    public static Activity? SetBookId(this Activity? activity, Guid id)
    {
        activity?.SetTag(GlobalOTelTags.BookId, id);
        return activity;
    }
    
    public static Activity? SetBookIsbn(this Activity? activity, string isbn)
    {
        activity?.SetTag(GlobalOTelTags.BookIsbn, isbn);
        return activity;
    }
    
    public static Activity? SetBookTitle(this Activity? activity, string title)
    {
        activity?.SetTag(GlobalOTelTags.Title, title);
        return activity;
    }
    
    public static Activity? SetLoanId(this Activity? activity, Guid id)
    {
        activity?.SetTag(GlobalOTelTags.LoanId, id);
        return activity;
    }
    
    public static Activity? SetDateTimeOffset(this Activity? activity, DateTimeOffset offset)
    {
        activity?.SetTag(GlobalOTelTags.UtcDateTimeOffset, offset);
        return activity;
    }

    public static Activity? SetRoutingKey(this Activity? activity, string routingKey)
    {
        activity?.SetTag("routingKey", routingKey);
        return activity;
    }
    
    // TODO: add rest of OTel tags.
}