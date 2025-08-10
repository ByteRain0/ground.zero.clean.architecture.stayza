using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Stayza.Infrastructure.Messaging.Subscriber;

public class WorkerService : BackgroundService
{
    private readonly ILogger<WorkerService> _logger;
    
    private readonly RabbitMqReceiver _rabbitMqReceiver;
    
    public WorkerService(
        RabbitMqReceiver rabbitMqReceiver,
        ILogger<WorkerService> logger)
    {
        _logger = logger;
        _rabbitMqReceiver = rabbitMqReceiver;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("Registering RabbitMQ listeners");
        _rabbitMqReceiver.RegisterListeners();
        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        if (_rabbitMqReceiver is not null)
        {
            _rabbitMqReceiver.Dispose();
        }
        
        return Task.CompletedTask;
    }
}