using Asp.Versioning;
using O9d.AspNet.FluentValidation;
using Stayza.Application.Books;
using Stayza.Application.Books.Commands;
using Stayza.Domain.Books;
using Stayza.Domain.Loans;
using Stayza.Web.Infrastructure.Endpoints;
using static Stayza.Web.Infrastructure.Endpoints.Constants.ContentTypes;

namespace Stayza.Web.Endpoints.Books;

public class BooksEndpoints : IEndpointsDefinition
{
    public static void ConfigureEndpoints(IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1.0))
            .ReportApiVersions()
            .Build();

        var group = app.MapGroup("api/v{version:apiVersion}/books/")
            .WithTags("books")
            .WithValidationFilter()
            .RequireAuthorization()
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(1.0);

        group.MapPost("", AddBook)
            .Accepts<AddBookCommand>(ApplicationJson)
            .Produces<Book>(201, ApplicationJson)
            .ProducesValidationProblem()
            .WithName("AddBook");

        group.MapGet("{id:guid}", GetBookById)
            .Produces(404)
            .Produces<Book>(200, ApplicationJson)
            .WithName("GetBookById");

        group.MapGet("{isbn}", GetBookByIsbn)
            .Produces(404)
            .Produces<Book>(200, ApplicationJson)
            .WithName("GetBookByIsbn");

        group.MapPost("{id:guid}/retire", RetireBook)
            .Produces(404)
            .Produces<Book>()
            .WithName("RetireBook");

        group.MapPost("{id:guid}/copies", AddBookCopy)
            .Produces(404)
            .Produces<BookCopy>()
            .WithName("AddBookCopy");

        group.MapDelete("{bookId:guid}/{bookCopyId:guid}", RemoveBookCopy)
            .Produces(404)
            .Produces<Book>()
            .WithName("RemoveBookCopy");
    }

    private static async Task<IResult> AddBook(
        [Validate] AddBookCommand command,
        BooksService service,
        LinkGenerator linkGenerator,
        HttpContext httpContext)
    {
        var book = await service.AddBook(command);
        var path = linkGenerator.GetUriByName(
            httpContext,
            endpointName: "GetBookById",
            new {id = book.Id});
        return Results.Created(path, book);
    }

    private static async Task<IResult> GetBookById(
        Guid id,
        BooksService service,
        CancellationToken cancellationToken) =>
        Results.Ok(await service.GetBookById(
            id: id,
            cancellationToken: cancellationToken));

    private static async Task<IResult> GetBookByIsbn(
        string isbn,
        BooksService service,
        CancellationToken cancellationToken) =>
        Results.Ok(await service.GetBookByIsbn(
            isbn: isbn,
            cancellationToken: cancellationToken));

    private static async Task<IResult> RetireBook(
        Guid id,
        BooksService service)
        => Results.Ok(await service.Retire(new RetireBookCommand(BookId: id)));

    private static async Task<IResult> AddBookCopy(
        Guid id,
        BooksService service)
        => Results.Ok(await service.AddBookCopy(new AddBookCopyCommand(BookId: id)));

    private static async Task<IResult> RemoveBookCopy(
        Guid bookId,
        Guid bookCopyId,
        BooksService service) =>
        Results.Ok(await service.RemoveBookCopy(
            new RemoveBookCopyCommand(
                BookId: bookId,
                BookCopyId: bookCopyId)));
}