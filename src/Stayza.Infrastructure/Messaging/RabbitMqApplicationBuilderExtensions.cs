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
        builder.Services
            .SetUpRabbitMQ(builder.Configuration)
            .AddSingleton<RabbitMQReceiver>()
            .RegisterListeners()
            .AddSingleton<IMessageProducer, MessageProducer>();
        
        return builder;
    }

    private static IServiceCollection SetUpRabbitMQ(
        this IServiceCollection services, 
        IConfiguration config)
    {
        // add the settings for later use by other classes via injection
        // might use IOptions pattern together with snapshots for realtime update of the settings
        var configSection = config.GetSection("RabbitMQSettings");
        var settings = new RabbitMQSettings();
        configSection.Bind(settings);
        services.AddSingleton<RabbitMQSettings>(settings);

        // As the connection factory is disposable, need to ensure container disposes of it when finished
        services.AddSingleton<IConnectionFactory>(_ => new ConnectionFactory
        {
            DispatchConsumersAsync = true,
            // Depending on the personal preference you can use either the username/password approach
            // or via connection string
            //HostName = settings.HostName,
            //UserName = settings.UserName,
            //Password = settings.Password
            Uri = new Uri(settings.ConnectionString)
        });

        services.AddSingleton<ModelFactory>();
        services.AddSingleton(sp => sp.GetRequiredService<ModelFactory>().CreateChannel());

        return services;
    }

    private static IServiceCollection RegisterListeners(this IServiceCollection services)
    {
        services.AddSingleton<IListener, ReservationFulfilledEventHandler>();
        services.AddSingleton<IListener, BookReturnedEventHandler>();
        services.AddSingleton<IListener, BookCopyLoanedEventHandler>();

        return services;
    }

    private class ModelFactory : IDisposable
    {
        private readonly IConnection _connection;
        
        private readonly RabbitMQSettings _settings;
        
        public ModelFactory(
            IConnectionFactory connectionFactory,
            RabbitMQSettings settings)
        {
            _settings = settings;
            _connection = connectionFactory.CreateConnection();
        }

        public IModel CreateChannel()
        {
            var channel = _connection.CreateModel();
            
            channel.ExchangeDeclare(
                exchange: _settings.ExchangeName, 
                type: _settings.ExchangeType);
            
            return channel;
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}