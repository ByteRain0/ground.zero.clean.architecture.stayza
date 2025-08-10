namespace Stayza.Infrastructure.Messaging;

public class RabbitMQSettings
{
    public string HostName { get; set; }

    public string ExchangeName { get; set; }

    public string ExchangeType { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public string ConnectionString { get; set; }

    public string AppPrefix { get; set; } = "stazya.queue.";
}