using System.Text.Json;
using Microsoft.Extensions.Logging;
using Stayza.Core.Messaging;
using Stayza.Domain.Loans;
using Stayza.Domain.Loans.Events;

namespace Stayza.Application.Loans.EventHandlers;

public class BookReturnedEventHandler(
    ILogger<BookReturnedEventHandler> logger,
    ILoansRepository repository,
    TimeProvider timeProvider) : IListener
{
    public string RoutingKey => RoutingKeys.BookCopyReturnedTopic
        .ReplaceBookCopyIdPlaceholderWith("*")
        .ReplaceLoanIdPlaceholderWith("*");

    public async Task ProcessMessage(Message message, string routingKey)
    {
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