using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Stayza.Application.Loans.EventHandlers;
using Stayza.Core.Messaging;
using Stayza.Infrastructure.Messaging.Producer;
using Stayza.Infrastructure.Messaging.Subscriber;

namespace Stayza.Infrastructure.Messaging;

public static class RabbitMqApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddAsyncMessagingUsingRabbitMq(
        this IHostApplicationBuilder builder)
    {
        var configSection = builder.Configuration.GetSection("RabbitMQSettings");
        var settings = new RabbitMQSettings();
        configSection.Bind(settings);
        builder.Services.AddSingleton(settings);
        
        // As the connection factory is disposable, need to ensure container disposes of it when finished
        builder.Services.AddSingleton<IConnectionFactory>(_ => new ConnectionFactory
        {
            DispatchConsumersAsync = true,
            // Depending on the personal preference you can use either the username/password approach
            // or via connection string
            //HostName = settings.HostName,
            //UserName = settings.UserName,
            //Password = settings.Password
            Uri = new Uri(settings.ConnectionString)
        });

        builder.Services.AddSingleton<ModelFactory>();
        builder.Services.AddSingleton(sp => sp.GetRequiredService<ModelFactory>().CreateChannel());
        
        builder.Services.AddSingleton<RabbitMqReceiver>();
        builder.Services.AddSingleton<IMessageProducer, MessageProducer>();
        
        builder.Services.AddSingleton<IListener, ReservationFulfilledEventHandler>();
        builder.Services.AddSingleton<IListener, BookReturnedEventHandler>();
        builder.Services.AddSingleton<IListener, BookCopyLoanedEventHandler>();
        
        builder.Services.AddHostedService<WorkerService>();
        
        return builder;
    }
}