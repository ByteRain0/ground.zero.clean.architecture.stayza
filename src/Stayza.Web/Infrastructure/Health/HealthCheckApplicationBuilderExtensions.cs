using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Stayza.Infrastructure.Persistence;

namespace Stayza.Web.Infrastructure.Health;

public static class HealthCheckApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"])
            // EF Core / Database check
            .AddDbContextCheck<ApplicationDbContext>(
                name: "database",
                failureStatus: HealthStatus.Unhealthy
            )
            // RabbitMQ check
            .AddRabbitMQ(
                rabbitConnectionString: builder.Configuration["RabbitMQSettings:ConnectionString"],
                name: "rabbitmq",
                failureStatus: HealthStatus.Unhealthy
            )
            // External REST API check
            .AddUrlGroup(
                uri: new Uri($"{builder.Configuration["Notifications:BaseUrl"]!}/health"),
                name: "notification-api",
                failureStatus: HealthStatus.Unhealthy
            )
            .AddCheck<CustomHealthCheck>(
                name: "custom_check",
                failureStatus: HealthStatus.Unhealthy);
        
        return builder;
    }
    
    public static WebApplication MapDefaultHealthEndpoints(this WebApplication app)
    {
        // Adding health checks endpoints to applications in non-development environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
        if (app.Environment.IsDevelopment())
        {
            // All health checks must pass for app to be considered ready to accept traffic after starting
            // All health checks must pass for readiness
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

            // Only "live" tag for liveness probe
            app.MapHealthChecks("/alive", new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live")
            });
        }

        return app;
    }
}