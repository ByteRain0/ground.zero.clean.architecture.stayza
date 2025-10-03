using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stayza.Core.Messaging;
using Stayza.Core.Telemetry;
using Stayza.Domain.Loans;
using Stayza.Domain.Loans.Events;

namespace Stayza.Application.Loans.EventHandlers;

public class BookCopyLoanedEventHandler
    (IServiceScopeFactory serviceScopeFactory): IListener
{
    public string RoutingKey => RoutingKeys.BookCopyLoanedTopic
        .ReplaceBookCopyIdPlaceholderWith("*");
    
    public Task ProcessMessage(Message message, string routingKey)
    {
        using var activity = RunTimeDiagnosticConfig.Source.StartActivity("Handling book loaned event");
        
        using var scope = serviceScopeFactory.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ReservationFulfilledEventHandler>>();
        var notificationService = scope.ServiceProvider.GetRequiredService<IUserNotificationService>();
        
        var incomingEvent = JsonSerializer.Deserialize<BookLoanedEvent>(message.Body);

        if (incomingEvent is null)
        {
            logger.LogCritical(
                "Issues encountered processing events of type : {eventType}.",
                nameof(ReservationFulfilledEvent));
        }

        notificationService.NotifyUser(incomingEvent.UserId, nameof(BookLoanedEvent));
        
        return Task.CompletedTask;
    }
}