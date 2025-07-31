using Asp.Versioning;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Stayza.Web.Infrastructure.Swagger;

internal static class SwaggerApplicationBuilderExtensions
{
    internal static IHostApplicationBuilder AddConfiguredSwagger(this IHostApplicationBuilder builder)
    {
        builder
            .Services
            .AddOpenApi()
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

        return builder;
    }

    internal static WebApplication MapConfiguredSwagger(this WebApplication app)
    {
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

        return app;
    }
}