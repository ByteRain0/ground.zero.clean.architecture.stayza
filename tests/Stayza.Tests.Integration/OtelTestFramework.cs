using System.Diagnostics;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PracticalOtel.xUnit.OpenTelemetry;
using Stayza.Core.Telemetry;
using Xunit.Abstractions;

[assembly: TestFramework("Stayza.Tests.Integration.OtelTestFramework", "Stayza.Tests.Integration")]

namespace Stayza.Tests.Integration;

public class OtelTestFramework : TracedTestFramework
{
    public static TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

    public static ActivitySource Source = new ActivitySource(name: "Stayza.Tests.Integration", version: "1.0");
    
    public OtelTestFramework(IMessageSink messageSink) : base(messageSink)
    {
        traceProviderSetup = tpb => {
            tpb
                .ConfigureResource(resource => 
                    resource.AddService("Stayza.Tests.Integration"))
                .AddSource(Source.Name)
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddSource(RunTimeDiagnosticConfig.Source.Name)
                .AddOtlpExporter();
        };
    }
}