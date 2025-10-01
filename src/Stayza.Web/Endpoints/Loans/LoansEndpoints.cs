using System.Security.Claims;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Stayza.Application.Loans;
using Stayza.Application.Loans.Commands;
using Stayza.Application.Loans.Queries;
using Stayza.Core.PagingAndSorting;
using Stayza.Domain.Loans;
using Stayza.Web.Infrastructure.Endpoints;
using Stayza.Web.Infrastructure.Session;
using static Stayza.Web.Infrastructure.Endpoints.Constants.ContentTypes;

namespace Stayza.Web.Endpoints.Loans;

internal class LoansEndpoints : IEndpointsDefinition
{
    public static void ConfigureEndpoints(IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1.0))
            .ReportApiVersions()
            .Build();

        var bookCopiesGroup = app.MapGroup("api/v{version:apiVersion}/book-copies/")
            .WithTags("copies")
            .WithValidationFilter()
            .RequireAuthorization()
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(1.0)
            .ProducesProblem(statusCode: 400)
            .ProducesProblem(statusCode: 500);

        var loansGroup = app.MapGroup("api/v{version:apiVersion}/loans")
            .WithTags("loans")
            .WithValidationFilter()
            .RequireAuthorization()
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(1.0)
            .ProducesProblem(statusCode: 400)
            .ProducesProblem(statusCode: 500);

        bookCopiesGroup.MapPost("{id:guid}/reservations", ReserveBookCopy)
            .Produces(statusCode: 404)
            .Produces<Reservation>(statusCode: 201, ApplicationJson)
            .WithName("ReserveBookCopy");

        bookCopiesGroup.MapPut("{id:guid}/reservations/{reservationId:guid}/cancel", CancelBookReservation)
            .Produces(statusCode: 404)
            .Produces<Reservation>(statusCode: 200, ApplicationJson)
            .WithName("CancelBookReservation");

        bookCopiesGroup.MapPost("{id:guid}/loans", LoanBookCopy)
            .Produces(statusCode: 404)
            .Produces<Loan>(statusCode: 201, ApplicationJson)
            .WithName("LoanBookCopy");

        bookCopiesGroup.MapPut("{id:guid}/loans/return", ReturnBookCopy)
            .Produces(statusCode: 404)
            .Produces<Loan>(statusCode: 200, ApplicationJson)
            .WithName("ReturnBookCopy");

        loansGroup.MapGet("{id:guid}", GetLoanById)
            .Produces(statusCode: 404)
            .Produces<Loan>(statusCode: 200, ApplicationJson)
            .WithName("GetLoanById");

        loansGroup.MapGet("/personal", GetLoansByUserId)
            .Produces(statusCode: 404)
            .Produces<PagedList<Loan>>(statusCode: 200, ApplicationJson)
            .WithName("LoansByUserId")
            .CacheOutput("GetLoansByUserId");
    }

    private static async Task<IResult> ReserveBookCopy(
        [FromRoute] Guid id,
        ClaimsPrincipal claimsPrincipal,
        [FromServices] LoansService service,
        [FromServices] SessionAccessorService sessionAccessorService,
        [FromServices] LinkGenerator linkGenerator,
        HttpContext httpContext)
    {
        var reservation = await service.ReserveBookCopy(
            new ReserveBookCopyCommand(
                BookCopyId: id,
                UserId: await sessionAccessorService.GetUserId(claimsPrincipal)));

        var path = linkGenerator.GetUriByName(
            httpContext,
            endpointName: "GetBookCopyReservationById",
            new {id = reservation.Id});

        return Results.Created(path, reservation);
    }

    private static async Task<IResult> CancelBookReservation(
        [FromRoute] Guid id,
        [FromRoute] Guid reservationId,
        [FromBody] string reason, // Not really best example but will prove the point :P
        [FromServices] LoansService service) =>
        Results.Ok(await service.CancelReservation(new CancelReservationCommand(
            BookCopyId: id,
            ReservationId: reservationId,
            Reason: reason)));

    private static async Task<IResult> LoanBookCopy(
        [FromRoute] Guid id,
        ClaimsPrincipal claimsPrincipal,
        [FromServices] LoansService service,
        [FromServices] LinkGenerator linkGenerator,
        [FromServices] SessionAccessorService sessionAccessorService,
        HttpContext httpContext)
    {
        var loan = await service.StartLoan(new StartLoanCommand(
            BookCopyId: id,
            UserId: await sessionAccessorService.GetUserId(claimsPrincipal)));

        var path = linkGenerator.GetUriByName(httpContext, endpointName: "GetLoanById", new {id = loan.Id});
        return Results.Created(path, loan);
    }

    private static async Task<IResult> ReturnBookCopy(
        [FromRoute] Guid id,
        ClaimsPrincipal claimsPrincipal,
        [FromServices] SessionAccessorService sessionAccessorService,
        [FromServices] LoansService service) =>
        Results.Ok(await service.ReturnBookCopy(
            new ReturnBookCopyCommand(
                BookCopyId: id,
                UserId: await sessionAccessorService.GetUserId(claimsPrincipal))));

    private static async Task<IResult> GetLoanById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken,
        [FromServices] LoansService service) =>
        Results.Ok(await service.GetLoanById(
            id: id,
            cancellationToken: cancellationToken));

    private static async Task<IResult> GetLoansByUserId(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? sortColumn,
        [FromQuery] SortOrder? sortOrder,
        CancellationToken cancellationToken,
        ClaimsPrincipal claimsPrincipal,
        [FromServices] SessionAccessorService sessionAccessorService,
        [FromServices] LoansService service
    ) =>
        Results.Ok(await service.GetLoans(new GetLoansQuery
        {
            Page = page ?? 1,
            PageSize = pageSize ?? 50,
            SortColumn = sortColumn ?? nameof(Loan.ReturnDate),
            SortOrder = sortOrder ?? SortOrder.Descending,
            UserId = await sessionAccessorService.GetUserId(claimsPrincipal)
        }, cancellationToken));
}