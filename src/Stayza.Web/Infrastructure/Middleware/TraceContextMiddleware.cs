using System.Diagnostics;
using OpenTelemetry;
using Stayza.Core.Telemetry;

namespace Stayza.Web.Infrastructure.Middleware;

public class TraceContextMiddleware
{
    private readonly RequestDelegate _next;

    public TraceContextMiddleware(
        RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Extract the parent context from the incoming request
        var parentContext = RunTimeDiagnosticConfig.Propagator.Extract(default, context.Request, ExtractTraceContextFromRequest);

        Baggage.Current = parentContext.Baggage;

        using var activity = RunTimeDiagnosticConfig.Source.StartActivity(
            $"{context.Request.Method} {context.Request.Path}",
            ActivityKind.Server,
            parentContext.ActivityContext);

        await _next(context);
    }

    private static IEnumerable<string> ExtractTraceContextFromRequest(HttpRequest request, string key)
    {
        if (request.Headers.TryGetValue(key, out var values))
        {
            return values;
        }
        return Enumerable.Empty<string>();
    }
}