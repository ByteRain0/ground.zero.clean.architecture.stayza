using Asp.Versioning;
using Microsoft.Extensions.Options;
using Stayza.Application;
using Stayza.Domain.Users;
using Stayza.Infrastructure;
using Stayza.Infrastructure.Persistence;
using Stayza.Web.Infrastructure.Endpoints;
using Stayza.Web.Infrastructure.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddApplication()
    .AddInfrastructure();

builder
    .Services
    .AddEndpointsApiExplorer()
    .AddApiVersioning(config =>
    {
        config.DefaultApiVersion = new ApiVersion(1.0);
        config.AssumeDefaultVersionWhenUnspecified = true;
        config.ApiVersionReader = new UrlSegmentApiVersionReader();
        config.ReportApiVersions = true;
    })
    .AddApiExplorer(config =>
    {
        config.GroupNameFormat = "'v'VVV";
        config.SubstituteApiVersionInUrl = true;
    })
    .EnableApiVersionBinding();

builder.Services
    .AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>()
    .AddSwaggerGen();

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
        foreach (var description in app.DescribeApiVersions())
        {
            config.SwaggerEndpoint(
                url: $"/swagger/{description.GroupName}/swagger.json",
                name: description.GroupName);
        }

        config.RoutePrefix = string.Empty;
    });

app.Run();