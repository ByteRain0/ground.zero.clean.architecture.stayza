namespace Stayza.Core.Messaging;

public interface IListener
{
    public string RoutingKey { get; }

    Task ProcessMessage(Message message, string routingKey);
}