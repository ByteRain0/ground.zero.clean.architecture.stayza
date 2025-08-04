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
    
    public static Activity? SetDateTimeOffset(this Activity? activity, DateTimeOffset offset)
    {
        activity?.SetTag(GlobalOTelTags.UtcDateTimeOffset, offset);
        return activity;
    }
    
    // TODO: add rest of OTel tags.
}