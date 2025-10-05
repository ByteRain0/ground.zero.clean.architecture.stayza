using System.Diagnostics;
using OpenTelemetry.Context.Propagation;

namespace Stayza.App.Infrastructure;

public static class RunTimeDiagnosticConfig
{
    public const string ServiceName = "dotnet-web-app";

    public static string ServiceVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown";
    
    public static ActivitySource Source = new(ServiceName);
    
    public static TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

}

// Custom telemetry setup for the FE app.