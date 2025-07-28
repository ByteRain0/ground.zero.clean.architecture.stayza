using System.Text.Json;
using Microsoft.Extensions.Logging;
using Stayza.Core.Messaging;
using Stayza.Domain.Loans;
using Stayza.Domain.Loans.Events;

namespace Stayza.Application.Loans.EventHandlers;

public class ReservationFulfilledEventHandler(
        IUserNotificationService userNotificationService,
        ILogger<ReservationFulfilledEventHandler> logger): IListener
{
    // Subscribe to all notifications for all book copies and reservations.
    public string RoutingKey => RoutingKeys.BookCopyReservationFulfilledTopic
        .ReplaceBookCopyIdPlaceholderWith("*")
        .ReplaceReservationIdPlaceholderWith("*");

    public async Task ProcessMessage(Message message, string routingKey)
    {
        var incomingEvent = JsonSerializer.Deserialize<ReservationFulfilledEvent>(message.Body);

        if (incomingEvent is null)
        {
            logger.LogCritical("Issues encountered processing events of type : {eventType}",
                nameof(ReservationFulfilledEvent));
        }

        await userNotificationService.NotifyUser(incomingEvent.UserId, nameof(ReservationFulfilledEvent));
    }
}