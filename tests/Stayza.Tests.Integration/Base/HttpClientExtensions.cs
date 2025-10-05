using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace Stayza.Tests.Integration.Base;

public static class HttpClientExtensions
{
    public static void InjectTraceContext(this HttpClient client, Activity activity)
    {
        if (Activity.Current is null)
        {
            return;
        }
        
        // Remove existing trace headers if any, to avoid duplicates
        client.DefaultRequestHeaders.Remove("traceparent");
        client.DefaultRequestHeaders.Remove("tracestate");
        // Add any other headers your propagator uses if needed

        OtelTestFramework.Propagator.Inject(
            new PropagationContext(activity.Context, Baggage.Current),
            client,
            (c, key, value) =>
            {
                if (!c.DefaultRequestHeaders.Contains(key))
                    c.DefaultRequestHeaders.Add(key, value);
            });
    }
}