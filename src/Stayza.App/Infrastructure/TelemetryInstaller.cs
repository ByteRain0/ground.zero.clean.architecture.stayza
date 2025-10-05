using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Stayza.Infrastructure.Telemetry;

namespace Stayza.App.Infrastructure;

public static class TelemetryInstaller
{
    public static WebApplicationBuilder AddTelemetry(this WebApplicationBuilder builder)
    {
        OpenTelemetrySettings telemetrySettings = builder
            .Configuration
            .GetSection(nameof(OpenTelemetrySettings))
            .Get<OpenTelemetrySettings>()!;
        
        if (!telemetrySettings.Enabled)
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

        builder.Logging.AddOpenTelemetry(config =>
        {
            var resourceBuilder = ResourceBuilder.CreateDefault();
            configureResource(resourceBuilder);
            config.SetResourceBuilder(resourceBuilder);
            config.IncludeScopes = true;
            config.IncludeFormattedMessage = true;
            config.ParseStateValues = true;
            config.AddOtlpExporter(exporterOptions =>
            {
                exporterOptions.Endpoint = telemetrySettings.TracesEndpoint;
            });
        });

        builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(configureResource)
            .WithTracing(traceProviderBuilder =>
                traceProviderBuilder
                    .AddSource(RunTimeDiagnosticConfig.Source.Name)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
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
                    .AddMeter("System.Runtime",
                        "Microsoft.AspNetCore.Hosting",
                        "Microsoft.AspNetCore.Server.Kestrel"));

        return builder;
    }
}