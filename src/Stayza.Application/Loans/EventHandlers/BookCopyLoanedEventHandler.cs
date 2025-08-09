using Stayza.Core.Messaging;
using Stayza.Core.Telemetry;

namespace Stayza.Application.Loans.EventHandlers;

public class BookCopyLoanedEventHandler : IListener
{
    public string RoutingKey => RoutingKeys.BookCopyLoanedTopic
        .ReplaceBookCopyIdPlaceholderWith("*");
    
    public Task ProcessMessage(Message message, string routingKey)
    {
        using var activity = RunTimeDiagnosticConfig.Source.StartActivity("Handling book loaned event");
        Console.WriteLine("You might want to do something here :) ");
        return Task.CompletedTask;
    }
}