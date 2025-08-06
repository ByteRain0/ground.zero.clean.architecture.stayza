using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PracticalOtel.xUnit.OpenTelemetry;
using Stayza.Core.Telemetry;
using Xunit.Abstractions;

namespace Stayza.Tests.Integration;

public class OtelTestFramework : TracedTestFramework
{
    public static TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
    
    public OtelTestFramework(IMessageSink messageSink) : base(messageSink)
    {
        traceProviderSetup = tpb => {
            tpb
                .ConfigureResource(resource => resource.AddService("Stayza.Tests.Integration"))
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddSource(RunTimeDiagnosticConfig.Source.Name)
                .AddOtlpExporter();
        };
    }
}