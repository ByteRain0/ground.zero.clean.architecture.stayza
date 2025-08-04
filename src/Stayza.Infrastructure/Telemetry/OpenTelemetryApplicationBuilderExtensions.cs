using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Stayza.Core.Telemetry;
using Stayza.Infrastructure.Telemetry.Exporters;
using Stayza.Infrastructure.Telemetry.Samplers;
using BatchActivityExportProcessor = OpenTelemetry.BatchActivityExportProcessor;

namespace Stayza.Infrastructure.Telemetry;

internal static class OpenTelemetryApplicationBuilderExtensions
{
    internal static IHostApplicationBuilder AddCustomTelemetry(this IHostApplicationBuilder builder)
    {
        OpenTelemetrySettings telemetrySettings = builder
            .Configuration
            .GetSection(nameof(OpenTelemetrySettings))
            .Get<OpenTelemetrySettings>()!;
        
        if (telemetrySettings.Disabled)
        {
            // In case the app is running in Test mode
            return builder;
        }
        
        Action<ResourceBuilder> configureResource = resourceBuilder =>
        {
            resourceBuilder.AddService(
                serviceName: RunTimeDiagnosticConfig.ServiceName,
                serviceVersion: RunTimeDiagnosticConfig.ServiceVersion,
                serviceInstanceId: Environment.MachineName);
            resourceBuilder.AddAttributes(telemetrySettings.GetOtelAttributes());
        };
        
        
        // Alternative build set-up in case you are not using Serilog with it's sinks.
        builder.Logging.AddOpenTelemetry(config =>
        {
            var resourceBuilder = ResourceBuilder.CreateDefault();
            //configureResource(resourceBuilder);
            config.SetResourceBuilder(resourceBuilder);
            config.IncludeScopes = true;
            config.IncludeFormattedMessage = true;
            config.ParseStateValues = true;
            config.AddOtlpExporter(exporterOptions =>
            {
                exporterOptions.Endpoint = telemetrySettings.TracesEndpoint;
            });
        });
        
        builder.Services.AddSingleton(sp => 
            new PremiumUsersCheckSampler<AlwaysOffSampler>(
                new UserTiers(),
                sp.GetRequiredService<IHttpContextAccessor>(),
                sp.GetRequiredService<AlwaysOffSampler>()));
        
        builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(configureResource)
            .WithTracing(traceProviderBuilder =>
                traceProviderBuilder
                    .AddSource(RunTimeDiagnosticConfig.Source.Name)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    // .SetSampler<PremiumUsersCheckSampler<AlwaysOffSampler>>()
                    .AddProcessor(new BatchActivityExportProcessor(new SimpleLinkExporter()))
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = telemetrySettings.TracesEndpoint;
                        options.Protocol = OtlpExportProtocol.Grpc;
                    })
            )
            .WithMetrics(meterProviderBuilder =>
                meterProviderBuilder
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = telemetrySettings.TracesEndpoint;
                        options.Protocol = OtlpExportProtocol.Grpc;
                    })
                    .AddMeter(
                        "System.Runtime",
                        "Microsoft.AspNetCore.Hosting",
                        "Microsoft.AspNetCore.Server.Kestrel",
                        RunTimeDiagnosticConfig.Meter.Name
                    ));
        

        return builder;
    }
}