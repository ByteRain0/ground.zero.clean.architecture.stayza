using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Stayza.Infrastructure.ExternalConfigurations;
using Stayza.Web.Infrastructure.Endpoints;

namespace Stayza.Web.Endpoints.Configuration;

internal class ConfigurationEndpoints : IEndpointsDefinition
{
    public static void ConfigureEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/configurations", async (
                [FromServices] IOptionsSnapshot<AppConfiguration> options,
                [FromServices] IVariantFeatureManager featureManager
            )
            =>
        {
            var test = await featureManager.IsEnabledAsync("UserTypeFiltering");
            
            if (await featureManager.IsEnabledAsync("AllowConfigurationsRetrieval"))
            {
                return Results.Ok(options.Value);
            }

            return Results.Forbid();
        });
    }
}