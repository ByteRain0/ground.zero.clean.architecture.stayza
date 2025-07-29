using Microsoft.OpenApi.Models;
using Stayza.Application;
using Stayza.Domain.Users;
using Stayza.Infrastructure;
using Stayza.Infrastructure.Persistence;
using Stayza.Web.Infrastructure.Endpoints;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddApplication()
    .AddInfrastructure();

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(config =>
    {
        config.ExampleFilters();
        config.SwaggerDoc("v1", new OpenApiInfo {Title = "Stayza API", Version = "v1"});
    })
    .AddSwaggerExamplesFromAssemblyOf<Program>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.ApplyDbMigrations();
}

app.UseEndpoints<Program>();
app.MapIdentityApi<User>();

app
    .UseSwagger()
    .UseSwaggerUI(config =>
    {
        config.SwaggerEndpoint("/swagger/v1/swagger.json", "Stayza v1");
        config.RoutePrefix = string.Empty;
    });

app.Run();