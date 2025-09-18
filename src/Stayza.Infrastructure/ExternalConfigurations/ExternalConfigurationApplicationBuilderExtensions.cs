using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Steeltoe.Configuration.ConfigServer;

namespace Stayza.Infrastructure.ExternalConfigurations;


public static class ExternalConfigurationApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddExternalConfiguration(this WebApplicationBuilder builder)
    {
        ExternalConfigurationOptions externalConfigurationOptions = builder
            .Configuration
            .GetSection(nameof(ExternalConfigurationOptions))
            .Get<ExternalConfigurationOptions>()!;

        if (!externalConfigurationOptions.Enabled)
        {
            // In case the app is running in Test mode we don't really need external configuration
            return builder;
        }
        
        builder.AddConfigServer();
        builder.Services.AddOptions();
        builder.Services.Configure<AppConfiguration>(
            builder.Configuration.GetSection("AppConfiguration"));
        
        return builder;
    }
}