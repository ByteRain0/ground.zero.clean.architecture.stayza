using Notifications.Web;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/api/v1/notifications", (Notification notification) =>
{
    if (string.IsNullOrEmpty(notification.UserId))
    {
        return Results.BadRequest();
    }

    // Some mock result:
    return Results.Created("api/v1/notifications", notification);
});

app.Run();

namespace Notifications.Web
{
    public record Notification(
        string UserId,
        string Type);
}