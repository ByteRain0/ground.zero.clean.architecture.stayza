using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stayza.Domain.Loans;

namespace Stayza.Infrastructure.Notifications;

public static class NotificationsApplicationBuilderExtensions
{
    internal static IHostApplicationBuilder AddNotifications(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpClient<IUserNotificationService, UserNotificationsService>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["Notifications__URL"]
                                         ?? throw new Exception("Invalid notifications api configuration provided"));
        });

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            http.AddStandardResilienceHandler();
        });
        
        return builder;
    }
}