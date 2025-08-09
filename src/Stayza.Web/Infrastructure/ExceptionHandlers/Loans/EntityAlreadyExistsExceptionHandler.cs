using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Stayza.Core.Exceptions;
using Stayza.Core.Telemetry;

namespace Stayza.Web.Infrastructure.ExceptionHandlers.Loans;

public class EntityAlreadyExistsExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<EntityAlreadyExistsException> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not EntityAlreadyExistsException copyAlreadyLoanedException)
        {
            return false;
        }

        logger.LogError(
            exception: exception,
            message: copyAlreadyLoanedException.Message);

        Activity.Current?.AddExceptionAndFail(exception);
        
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Title = "ALREADY_EXISTS",
                Detail = copyAlreadyLoanedException.Message,
                Status = StatusCodes.Status400BadRequest
            }
        };
        return await problemDetailsService.TryWriteAsync(context);
    }
}