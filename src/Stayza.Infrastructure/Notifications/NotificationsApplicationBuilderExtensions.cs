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
            client.BaseAddress = new Uri(builder.Configuration["Notifications:BaseUrl"]
                                         ?? throw new Exception("Invalid notifications api url provided"));
        });
        
        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            http.AddStandardResilienceHandler(opts =>
            {
                // Example of how to reconfigure the settings
                opts.Retry.Delay = TimeSpan.FromSeconds(2);
                opts.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(60);
                
                opts.CircuitBreaker.FailureRatio = 0.3;
                opts.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(10);
            });
        });
        
        return builder;
    }
}