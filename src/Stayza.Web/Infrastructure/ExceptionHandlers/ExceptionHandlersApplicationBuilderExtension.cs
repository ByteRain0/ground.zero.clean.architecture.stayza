using Stayza.Web.Infrastructure.ExceptionHandlers.Loans;

namespace Stayza.Web.Infrastructure.ExceptionHandlers;

internal static class ExceptionHandlersApplicationBuilderExtension
{
    internal static IHostApplicationBuilder AddWebExceptionHandlers(this IHostApplicationBuilder builder)
    {
        // Add to all problem details the traceId of the request for easier debugging.
        builder.Services.AddProblemDetails(config =>
        {
            config.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
            };
        });
        
        builder.Services.AddExceptionHandler<BookCopyAlreadyLoanedExceptionHandler>();
        builder.Services.AddExceptionHandler<EntityAlreadyExistsExceptionHandler>();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        return builder;
    }
}