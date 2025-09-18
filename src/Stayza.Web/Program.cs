using Stayza.Application;
using Stayza.Domain.Users;
using Stayza.Infrastructure;
using Stayza.Infrastructure.ExternalConfigurations;
using Stayza.Infrastructure.Persistence;
using Stayza.Infrastructure.Persistence.DataSeed;
using Stayza.Infrastructure.Telemetry;
using Stayza.Web.Infrastructure.Endpoints;
using Stayza.Web.Infrastructure.ExceptionHandlers;
using Stayza.Web.Infrastructure.Health;
using Stayza.Web.Infrastructure.Middleware;
using Stayza.Web.Infrastructure.Session;
using Stayza.Web.Infrastructure.Swagger;
using TickerQ.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.AddBaseTelemetryConfiguration();

builder.AddExternalConfiguration();

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

app.UseTickerQ();

app.MapEndpointsFrom<Program>();
app.MapIdentityApi<User>();
app.MapConfiguredSwagger();
app.MapDefaultHealthEndpoints();


app.Run();