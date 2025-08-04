using System.Diagnostics;

namespace Stayza.Web.Infrastructure.Middleware;

internal class PerformanceMonitoringMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMonitoringMiddleware> _logger;
    private const long ThresholdMilliseconds = 500;

    public PerformanceMonitoringMiddleware(RequestDelegate next, ILogger<PerformanceMonitoringMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();

        if (stopwatch.ElapsedMilliseconds > ThresholdMilliseconds)
        {
            var requestPath = context.Request.Path;
            var method = context.Request.Method;

            _logger.LogWarning("Long running HTTP request: {Method} {Path} took {ElapsedMilliseconds} ms",
                method, requestPath, stopwatch.ElapsedMilliseconds);
        }
    }
}