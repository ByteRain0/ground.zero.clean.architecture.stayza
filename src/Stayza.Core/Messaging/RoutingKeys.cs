namespace Stayza.Core.Messaging;

public static class RoutingKeys
{
    public static string EventsTopicBase = "events.";
    
    public static string BookEvents = $"{EventsTopicBase}.[isbn]";
    
    public static string BookCopyEvents = $"{EventsTopicBase}.[bookCopyId]";
    
    public static string BookCopyLoanedTopic = $"{BookCopyEvents}.loaned";

    public static string BookCopyReturnedTopic = $"{BookCopyEvents}.[loanId].returned";

    public static string BookCopyLoanOverdueTopic = $"{BookCopyEvents}.[loanId].overdue";

    public static string BookCopyReservationExpiredTopic = $"{BookCopyEvents}.[reservationId].expired";
    
    public static string BookCopyReservationCancelledTopic = $"{BookCopyEvents}.[reservationId].cancelled";
    
    public static string BookCopyReservationFulfilledTopic = $"{BookCopyEvents}.[reservationId].fulfilled";

    public static string ReplaceIsbnPlaceholderWith(this string routingKey, string value)
        => routingKey.Replace("[isbn]", value.ToLowerInvariant());
    
    public static string ReplaceLoanIdPlaceholderWith(this string routingKey, string value)
        => routingKey.Replace("[loanId]", value.ToString().ToLowerInvariant());
    
    public static string ReplaceBookCopyIdPlaceholderWith(this string routingKey, string value)
        => routingKey.Replace("[bookCopyId]", value.ToString().ToLowerInvariant());
    
    public static string ReplaceReservationIdPlaceholderWith(this string routingKey, string value)
        => routingKey.Replace("[reservationId]", value.ToString().ToLowerInvariant());
}