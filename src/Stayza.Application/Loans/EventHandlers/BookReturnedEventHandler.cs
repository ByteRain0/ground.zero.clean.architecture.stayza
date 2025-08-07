using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stayza.Core.Messaging;
using Stayza.Domain.Loans;
using Stayza.Domain.Loans.Events;

namespace Stayza.Application.Loans.EventHandlers;

public class BookReturnedEventHandler(
    IServiceScopeFactory serviceScopeFactory,
    TimeProvider timeProvider) : IListener
{
    public string RoutingKey => RoutingKeys.BookCopyReturnedTopic
        .ReplaceBookCopyIdPlaceholderWith("*")
        .ReplaceLoanIdPlaceholderWith("*");

    public async Task ProcessMessage(Message message, string routingKey)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ReservationFulfilledEventHandler>>();
        var repository = scope.ServiceProvider.GetRequiredService<ILoansRepository>();
        
        var incomingEvent = JsonSerializer.Deserialize<BookReturnedEvent>(message.Body);

        if (incomingEvent is null)
        {
            logger.LogCritical(
                "Issues encountered processing events of type : {eventType}.",
                nameof(ReservationFulfilledEvent));
        }

        var bookCopy = await repository.GetBookCopyById(
            id: incomingEvent!.BookCopyId,
            cancellationToken: CancellationToken.None);

        var nextReservation = bookCopy
            .PendingReservations
            .OrderBy(x => x.ReservedAt)
            .FirstOrDefault();

        if (nextReservation is null)
        {
            logger.LogInformation("No active reservation found for {bookCopyId}.", bookCopy.Id);
            return;
        }

        bookCopy.FulfillReservation(
            reservationId: nextReservation.Id,
            utcNow: timeProvider.GetUtcNow());

        await repository.UpdateBookCopy(bookCopy);
    }
}