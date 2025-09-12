using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Stayza.Web.Infrastructure.Middleware;

namespace Stayza.Tests.Unit.Middlewares;

public class PerformanceMonitoringMiddlewareTests
{
    [Fact(Skip = "Ignore for now until the refactor to use adapter for logs")]
    public async Task InvokeAsync_ShouldNotLog_WhenExecutionIsFast()
    {
        // Arrange
        var logger = Substitute.For<ILogger<PerformanceMonitoringMiddleware>>();
        var httpContext = new DefaultHttpContext();

        RequestDelegate fastDelegate = _ => Task.CompletedTask;

        var middleware = new PerformanceMonitoringMiddleware(fastDelegate, logger);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        logger.DidNotReceiveWithAnyArgs().LogWarning(default!, default!, default!, default!);
    }

    [Fact(Skip = "Ignore for now until the refactor to use adapter for logs")]
    public async Task InvokeAsync_ShouldLogWarning_WhenExecutionIsSlow()
    {
        // Arrange
        var logger = Substitute.For<ILogger<PerformanceMonitoringMiddleware>>();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Method = "GET";
        httpContext.Request.Path = "/test";

        RequestDelegate slowDelegate = async _ => await Task.Delay(600);

        var middleware = new PerformanceMonitoringMiddleware(slowDelegate, logger);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        logger.Received(1).LogWarning(
            "Long running HTTP request: {Method} {Path} took {ElapsedMilliseconds} ms",
            "GET",
            "/test",
            Arg.Any<long>());
    }
}