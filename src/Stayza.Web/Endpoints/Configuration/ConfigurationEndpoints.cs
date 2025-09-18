using Microsoft.Extensions.Options;
using Stayza.Infrastructure.ExternalConfigurations;
using Stayza.Web.Infrastructure.Endpoints;

namespace Stayza.Web.Endpoints.Configuration;

internal class ConfigurationEndpoints : IEndpointsDefinition
{
    public static void ConfigureEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/configurations", (IOptionsSnapshot<AppConfiguration> options)
            => Results.Ok(options.Value));
    }
}