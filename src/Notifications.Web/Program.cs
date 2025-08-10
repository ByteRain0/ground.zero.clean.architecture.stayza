using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using Notifications.Web;

var builder = WebApplication.CreateBuilder(args);

// Add basic health checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" });

var app = builder.Build();

// Notifications API endpoint
app.MapPost("/api/v1/notifications", (Notification notification) =>
{
    if (string.IsNullOrEmpty(notification.UserId))
    {
        return Results.BadRequest();
    }

    // Some mock result:
    return Results.Created("api/v1/notifications", notification);
});

// Health check endpoints
app.MapHealthChecks("/alive", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("live")
});

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            details = report.Entries.Select(e => new
            {
                key = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description
            })
        });
        await context.Response.WriteAsync(result);
    }
});

app.Run();

namespace Notifications.Web
{
    public record Notification(
        string UserId,
        string Type);
}