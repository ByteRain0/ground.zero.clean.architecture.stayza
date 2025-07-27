using Microsoft.AspNetCore.Mvc;
using Stayza.Application.Loans;
using Stayza.Application.Loans.Commands;
using Stayza.Application.Loans.Queries;
using Stayza.Core.Context;
using Stayza.Core.PagingAndSorting;
using Stayza.Domain.Loans;
using Stayza.Web.Infrastructure.Endpoints;
using static Stayza.Web.Infrastructure.Endpoints.Constants.ContentTypes;

namespace Stayza.Web.Endpoints.Loans;

public class LoansEndpoints : IEndpointsDefinition
{
    public static void ConfigureEndpoints(IEndpointRouteBuilder app)
    {
        var bookCopiesGroup = app.MapGroup("api/v1/book-copies/")
            .WithTags("copies")
            .WithValidationFilter();

        var loansGroup = app.MapGroup("api/v1/loans")
            .WithTags("loans")
            .WithValidationFilter();

        bookCopiesGroup.MapPost("{id:guid}/reservations", ReserveBookCopy)
            .Produces(404)
            .Produces<Reservation>(201, ApplicationJson)
            .WithName("ReserveBookCopy");
        
        bookCopiesGroup.MapPut("{id:guid}/reservations/{reservationId:guid}/cancel", CancelBookReservation)
            .Produces(404)
            .Produces<Reservation>(200, ApplicationJson)
            .WithName("CancelBookReservation");
        
        bookCopiesGroup.MapPost("{id:guid}/loans", LoanBookCopy)
            .Produces(404)
            .Produces<Loan>(201, ApplicationJson)
            .WithName("LoanBookCopy");
        
        bookCopiesGroup.MapPut("{id:guid}/loans/return", ReturnBookCopy)
            .Produces(404)
            .Produces<Loan>(200, ApplicationJson)
            .WithName("ReturnBookCopy");
        
        loansGroup.MapGet("{id:guid}", GetLoanById)
            .Produces(404)
            .Produces<Loan>(200, ApplicationJson)
            .WithName("GetLoanById");
        
        loansGroup.MapGet("/personal", GetLoansByUserId)
            .Produces(404)
            .Produces<PagedList<Loan>>(200, ApplicationJson)
            .WithName("LoansByUserId");
    }

    private static async Task<IResult> ReserveBookCopy(
        Guid id,
        LoansService service,
        IUserContext userContext,
        LinkGenerator linkGenerator,
        // The http context can be inferred from the UserContext if all you ever write are web api's.
        HttpContext httpContext)
    {
        var reservation = await service.ReserveBookCopy(
            new ReserveBookCopyCommand(
                BookCopyId: id,
                UserId: userContext.CurrentUserId()));
        var path = linkGenerator.GetUriByName(
            httpContext,
            endpointName: "GetBookCopyReservationById",
            new {id = reservation.Id});
        return Results.Created(path, reservation);
    }

    private static async Task<IResult> CancelBookReservation(
        Guid id,
        Guid reservationId,
        [FromBody] string reason,
        LoansService service) =>
        Results.Ok(await service.CancelReservation(new CancelReservationCommand(
            BookCopyId: id,
            ReservationId: reservationId,
            Reason: reason)));

    private static async Task<IResult> LoanBookCopy(
        Guid id,
        LoansService service,
        LinkGenerator linkGenerator,
        IUserContext userContext,
        // The http context can be inferred from the UserContext if all you ever write are web api's.
        HttpContext httpContext)
    {
        var loan = await service.StartLoan(new StartLoanCommand(
            BookCopyId: id,
            UserId: userContext.CurrentUserId()));
        var path = linkGenerator.GetUriByName(httpContext, endpointName: "GetLoanById", new {id = loan.Id});
        return Results.Created(path, loan);
    }

    private static async Task<IResult> ReturnBookCopy(
        Guid id,
        IUserContext userContext,
        LoansService service) =>
        Results.Ok(await service.ReturnBookCopy(
            new ReturnBookCopyCommand(
                BookCopyId: id,
                UserId: userContext.CurrentUserId())));

    private static async Task<IResult> GetLoanById(
        Guid id,
        CancellationToken cancellationToken,
        LoansService service) =>
        Results.Ok(await service.GetLoanById(id, cancellationToken));

    private static async Task<IResult> GetLoansByUserId(
        int? page,
        int? pageSize,
        string? sortColumn,
        SortOrder? sortOrder,
        CancellationToken cancellationToken,
        [FromServices] IUserContext userContext,
        [FromServices] LoansService service
    ) =>
        Results.Ok(await service.GetLoans(new GetLoansQuery
        {
            Page = page ?? 1,
            PageSize = pageSize ?? 50,
            SortColumn = sortColumn ?? nameof(Loan.ReturnDate),
            SortOrder = sortOrder ?? SortOrder.Descending,
            UserId = userContext.CurrentUserId()
        }, cancellationToken));
}