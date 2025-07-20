using Microsoft.AspNetCore.Mvc;
using Stayza.Application.Loans;
using Stayza.Application.Loans.Commands;
using Stayza.Core.Context;
using Stayza.Web.Infrastructure.Endpoints;

namespace Stayza.Web.Endpoints.Loans;

public class LoansEndpoints : IEndpointsDefinition
{
    public static void ConfigureEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/book-copies/")
            .WithTags("copies")
            .WithValidationFilter();
        
        // There can be 2 patterns here:
        // complex : api/v1/books/{bookId:guid}/{bookCopyId:guid}/reservations
        // simplified: api/v1/book-copies/{id:guid}/reservations

        group.MapPost("{id:guid}/reservations", ReserveBookCopy)
            .Produces(404)
            .Produces(201)
            .WithName("ReserveBookCopy");

        group.MapDelete("{id:guid}/reservations/{reservationId:guid}", CancelBookReservation)
            .Produces(404)
            .Produces(200)
            .WithName("CancelBookReservation");

        group.MapPost("{id:guid}/loans", StartLoan)
            .Produces(404)
            .Produces(201)
            .WithName("StartLoan");
        
        group.MapPost("{id:guid}/loans/{loanId:guid}", ReturnBookCopy)
            .Produces(404)
            .Produces(200)
            .WithName("ReturnBookCopy");
            
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
            new { id = reservation.Id });
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

    private static async Task<IResult> StartLoan(
        Guid id,
        LoansService service,
        LinkGenerator linkGenerator,
        IUserContext userContext,
        HttpContext httpContext)
    {
        var loan = await service.StartLoan(new StartLoanCommand(
            BookCopyId: id, 
            UserId: userContext.CurrentUserId()));
        var path = linkGenerator.GetUriByName(httpContext, endpointName: "GetLoanById", new { id = loan.Id });
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
}