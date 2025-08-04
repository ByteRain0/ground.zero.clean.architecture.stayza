using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stayza.Infrastructure.Auth;
using Stayza.Infrastructure.Messaging;
using Stayza.Infrastructure.Notifications;
using Stayza.Infrastructure.Persistence;
using Stayza.Infrastructure.Telemetry;

namespace Stayza.Infrastructure;

public static class AppllicationBuilderExtensions
{
    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);

        builder
            .AddAuth()
            .AddAsyncMessagingUsingRabbitMq()
            .AddNotifications()
            .AddPersistence();

        return builder;
    }
}