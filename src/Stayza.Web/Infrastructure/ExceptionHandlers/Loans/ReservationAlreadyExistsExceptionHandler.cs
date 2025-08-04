using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Stayza.Domain.Loans.Exceptions;

namespace Stayza.Web.Infrastructure.ExceptionHandlers.Loans;

internal class ReservationAlreadyExistsExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<ReservationAlreadyExistsExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ReservationAlreadyExistsException reservationAlreadyExists)
        {
            return false;
        }

        logger.LogError(
            exception: exception,
            message: "Reservation already exists for {userId} - {reservationId}.",
            args: [reservationAlreadyExists.UserId, reservationAlreadyExists.ReservationId]);

        httpContext.Response.StatusCode = StatusCodes.Status409Conflict;

        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Detail = "RESERVATION_EXISTS",
                Status = StatusCodes.Status409Conflict
            }
        };

        context.ProblemDetails.Extensions.Add("userId", reservationAlreadyExists.UserId);
        context.ProblemDetails.Extensions.Add("reservationId", reservationAlreadyExists.ReservationId);

        return await problemDetailsService.TryWriteAsync(context);
    }
}