using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Stayza.Domain.Loans.Exceptions;

namespace Stayza.Web.Infrastructure.ExceptionHandlers.Books;

/// <summary>
/// Alternatively you can have a single exception handler per aggregate root and switch through multiple exception types.
/// </summary>
/// <param name="problemDetailsService"></param>
/// <param name="logger"></param>
internal class BookCopyAlreadyLoanedExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<BookCopyAlreadyLoanedExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not BookCopyAlreadyLoanedException copyAlreadyLoanedException)
        {
            return false;
        }

        logger.LogError(
            exception: exception,
            message: "Book copy was already loaned to specific user. {userId}",
            args: copyAlreadyLoanedException.UserId);

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Detail = "BOOK_COPY_IS_LOANED_TO_USER",
                Status = StatusCodes.Status400BadRequest
            }
        };

        context.ProblemDetails.Extensions.Add("userId", copyAlreadyLoanedException.UserId);

        return await problemDetailsService.TryWriteAsync(context);
    }
}