using O9d.AspNet.FluentValidation;
using Stayza.Application.Books;
using Stayza.Application.Books.Commands;
using Stayza.Domain.BookAggregate;
using Stayza.Domain.BookCopyAggregate;
using Stayza.Web.Infrastructure.Endpoints;

namespace Stayza.Web.Endpoints.Books;

public class BooksEndpoints : IEndpointsDefinition
{
    public static void ConfigureEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/books")
            .WithTags("Books")
            .WithValidationFilter();

        group.MapPost("api/v1/books", AddBook)
            .Accepts<AddBookCommand>(Constants.ContentTypes.ApplicationJson)
            .Produces(201)
            .ProducesValidationProblem()
            .WithName("AddBook");

        group.MapGet("api/v1/books/{id:guid}", GetBookById)
            .Produces(404)
            .Produces<Book>()
            .WithName("GetBookById");

        group.MapGet("api/v1/books/{isbn}", GetBookByIsbn)
            .Produces(404)
            .Produces<Book>()
            .WithName("GetBookByIsbn");

        group.MapPost("api/v1/books/{id:guid}/retire", RetireBook)
            .Produces(404)
            .Produces<Book>()
            .WithName("RetireBook");

        group.MapPost("api/v1/books/{id:guid}", AddBookCopy)
            .Produces(404)
            .Produces<BookCopy>()
            .WithName("AddBookCopy");

        group.MapDelete("api/v1/books/{bookId:guid}/{bookCopyId:guid}", RemoveBookCopy)
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
        var path = linkGenerator.GetUriByName(httpContext, endpointName: "GetBookById", new { id = book.Id });
        return Results.Created(path, book);
    }

    private static async Task<IResult> GetBookById(
        Guid id,
        BooksService service,
        CancellationToken cancellationToken) =>
        Results.Ok(await service.GetBookById(id, cancellationToken));

    private static async Task<IResult> GetBookByIsbn(
        string isbn,
        BooksService service,
        CancellationToken cancellationToken) =>
        Results.Ok(await service.GetBookByIsbn(isbn, cancellationToken));

    private static async Task<IResult> RetireBook(Guid id, BooksService service)
        => Results.Ok(await service.Retire(new RetireBookCommand(BookId: id)));

    private static async Task<IResult> AddBookCopy(Guid id, BooksService service)
        => Results.Ok(await service.AddBookCopy(new AddBookCopyCommand(BookId: id)));

    private static async Task<IResult> RemoveBookCopy(
        Guid bookId,
        Guid bookCopyId,
        BooksService service) =>
        Results.Ok(await service.RemoveBookCopy(
            new RemoveBookCopyCommand(BookId: bookId, BookCopyId: bookCopyId)));
}