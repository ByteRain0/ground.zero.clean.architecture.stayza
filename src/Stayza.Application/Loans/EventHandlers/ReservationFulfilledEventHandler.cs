using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stayza.Core.Messaging;
using Stayza.Domain.Loans;
using Stayza.Domain.Loans.Events;

namespace Stayza.Application.Loans.EventHandlers;

public class ReservationFulfilledEventHandler(IServiceScopeFactory serviceScopeFactory) : IListener
{
    // Subscribe to all notifications for all book copies and reservations.
    public string RoutingKey => RoutingKeys.BookCopyReservationFulfilledTopic
        .ReplaceBookCopyIdPlaceholderWith("*")
        .ReplaceReservationIdPlaceholderWith("*");

    public async Task ProcessMessage(Message message, string routingKey)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var userNotificationService = scope.ServiceProvider.GetRequiredService<IUserNotificationService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ReservationFulfilledEventHandler>>();
        
        var incomingEvent = JsonSerializer.Deserialize<ReservationFulfilledEvent>(message.Body);

        if (incomingEvent is null)
        {
            logger.LogCritical("Issues encountered processing events of type : {eventType}",
                nameof(ReservationFulfilledEvent));
        }

        // Notify user that the book copy was returned and he can now loan it.
        await userNotificationService.NotifyUser(
            userId: incomingEvent!.UserId,
            eventType: "ReservationFulfilled");
    }
}