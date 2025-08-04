namespace Stayza.Web.Infrastructure.Middleware;

internal class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var requestPath = context.Request.Path;
        var method = context.Request.Method;

        _logger.LogInformation("Handling HTTP request: {Method} {Path}", method, requestPath);

        await _next(context);

        _logger.LogInformation("Finished handling HTTP request: {Method} {Path}", method, requestPath);
    }
}