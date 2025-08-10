using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stayza.Infrastructure.Auth;
using Stayza.Infrastructure.Messaging;
using Stayza.Infrastructure.Notifications;
using Stayza.Infrastructure.Persistence;

namespace Stayza.Infrastructure;

public static class ApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton(TimeProvider.System);
        
        builder
            .AddAuth()
            .AddAsyncMessagingUsingRabbitMq()
            .AddNotifications()
            .AddPersistence();

        return builder;
    }
}