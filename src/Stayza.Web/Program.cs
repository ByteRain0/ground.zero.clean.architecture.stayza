using Microsoft.OpenApi.Models;
using Stayza.Web.Infrastructure.Endpoints;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config =>
{
    config.ExampleFilters();
    config.SwaggerDoc("v1", new OpenApiInfo { Title = "Stayza API", Version = "v1" });
});
builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

var app = builder.Build();

// Register all endpoints defined in the Web Project
app.UseEndpoints<Program>();

app.UseSwagger();

app.UseSwaggerUI(config =>
{
    config.SwaggerEndpoint("/swagger/v1/swagger.json", "Stayza v1");
    config.RoutePrefix = string.Empty;
});

app.Run();