using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Stayza.Infrastructure.FeatureManagement.CustomFilters;

namespace Stayza.Infrastructure.FeatureManagement;

public static class FeatureManagementApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddCustomFeatureManagement(
        this IHostApplicationBuilder builder)
    {
        // In case you are ok with singleton injection.
        //builder.Services.AddFeatureManagement();
        
        // In case you need scoped services to determine the toggle
        builder.Services.AddScopedFeatureManagement()
            .AddFeatureFilter<UserTypeFeatureFilter>();
        
        builder.Services.Configure<ConfigurationFeatureDefinitionProviderOptions>(opts =>
        {
            // Merge feature definitions.
            opts.CustomConfigurationMergingEnabled = true;
        });

        
        return builder;
    }
}