using Stayza.Core.Messaging;

namespace Stayza.Application.Loans.EventHandlers;

public class BookCopyLoanedEventHandler : IListener
{
    public string RoutingKey => RoutingKeys.BookCopyLoanedTopic
        .ReplaceBookCopyIdPlaceholderWith("*");
    
    public Task ProcessMessage(Message message, string routingKey)
    {
        Console.WriteLine("You might want to do something here :) ");
        return Task.CompletedTask;
    }
}