using Stayza.Application;
using Stayza.Core.Telemetry;
using Stayza.Domain.Users;
using Stayza.Infrastructure;
using Stayza.Infrastructure.Persistence;
using Stayza.Infrastructure.Telemetry;
using Stayza.Web.Infrastructure.Endpoints;
using Stayza.Web.Infrastructure.ExceptionHandlers;
using Stayza.Web.Infrastructure.Health;
using Stayza.Web.Infrastructure.Middleware;
using Stayza.Web.Infrastructure.Seed;
using Stayza.Web.Infrastructure.Session;
using Stayza.Web.Infrastructure.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.AddBaseTelemetryConfiguration();

builder
    .AddApplication()
    .AddInfrastructure()
    .AddWebExceptionHandlers()
    .AddConfiguredSwagger()
    .AddDefaultHealthChecks();

builder.Services.AddScoped<SessionAccessorService>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.ApplyDbMigrations();
    await app.SeedTestUsers();
    app.MapOpenApi();
}

app
    .UseMiddleware<ActivityTracingMiddleware>()
    .UseMiddleware<LoggingMiddleware>()
    .UseMiddleware<PerformanceMonitoringMiddleware>();

app.MapEndpointsFrom<Program>();
app.MapIdentityApi<User>();
app.MapConfiguredSwagger();
app.MapDefaultHealthEndpoints();

app.Run();