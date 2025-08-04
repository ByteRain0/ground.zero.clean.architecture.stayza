using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Stayza.Web.Infrastructure.ExceptionHandlers;

public class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception occured");

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;


        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext()
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails()
            {
                Type = exception.GetType().Name,
                Title = "An error occured",
                Detail = exception.Message
            }
        });
    }
}