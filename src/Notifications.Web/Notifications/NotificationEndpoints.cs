using Notifications.Web.Infrastructure.EndpointConfigs;

namespace Notifications.Web.Notifications;

public class NotificationEndpoints : IEndpointsDefinition
{
    public static void ConfigureEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/notifications", (Notification notification) =>
        {
            if (string.IsNullOrEmpty(notification.UserId) || string.IsNullOrEmpty(notification.Type))
            {
                return Results.BadRequest();
            }

            return Results.Created("api/v1/notifications", notification);
        });
    }
}