using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace Stayza.App.Infrastructure;

public static class HttpClientExtensions
{
    public static void EnrichWithAppTraceData(this HttpClient client)
    {
        if (Activity.Current is null)
        {
            return;
        }
        
        var contextToInject = Activity.Current?.Context ?? default;
        
        RunTimeDiagnosticConfig.Propagator.Inject(
            new PropagationContext(contextToInject, Baggage.Current),
            client,
            InjectTraceContext);
        
        return;
        
        static void InjectTraceContext(HttpClient client, string key, string value)
            => client.DefaultRequestHeaders.Add(key, new[] {value});
    }
}