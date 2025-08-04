using System.Diagnostics;
using Stayza.Core.Telemetry;

namespace Stayza.Web.Infrastructure.Middleware;

internal class ActivityTracingMiddleware
{
    private readonly RequestDelegate _next;
    
    public ActivityTracingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        using var activity = RunTimeDiagnosticConfig.Source.StartActivity(
            $"Handling HTTP request: {context.Request.Method} {context.Request.Path}",
            ActivityKind.Server);

        // Optionally enrich with tags
        activity?.SetTag("http.method", context.Request.Method);
        activity?.SetTag("http.url", context.Request.Path);

        await _next(context);
    }
}