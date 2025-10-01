using System.Net;
using System.Net.Http.Json;
using Shouldly;
using Snapshooter.Xunit;
using Stayza.Application.Books.Commands;
using Stayza.Domain.Books;
using Stayza.Tests.Integration.Base;
using Stayza.Tests.Integration.Base.TestConstants;

namespace Stayza.Tests.Integration.Books.Tests;

[Collection("BooksTests")]
public class AddBookEndpointShould : IClassFixture<ApiFactory>
{
    private readonly HttpClient _stayzaWebClient;
    
    public AddBookEndpointShould(ApiFactory apiFactory)
    {
        _stayzaWebClient = apiFactory.CreateClient();
    }

    [Fact]
    public async Task Add_new_book()
    {
        // Arrange
        using var rootTestActivity = OtelTestFramework.Source.StartActivity();
        _stayzaWebClient.InjectTraceContext(rootTestActivity);
        await _stayzaWebClient.AuthenticateTestUser1();
        
        // Act
        var bookResponse = await _stayzaWebClient.PostAsJsonAsync("api/v1/books", new AddBookCommand(
            Title: Constants.Book.Title,
            Author: Constants.Book.Author,
            ISBN: Constants.Book.ISBN));
        
        // Assert
        bookResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var book = await bookResponse.Content.ReadFromJsonAsync<Book>();
        book!.Author.ShouldBe(Constants.Book.Author);
        book.Title.ShouldBe(Constants.Book.Title);
        book.ISBN.ShouldBe(Constants.Book.ISBN);
    }

    // Example of a flaky test that would be otherwise hard to catch.
    [Fact]
    public async Task Fail_if_book_with_same_isbn_exists()
    {
        // Arrange
        using var rootActivity = OtelTestFramework.Source.StartActivity();
        _stayzaWebClient.InjectTraceContext(rootActivity);
        
        await _stayzaWebClient.AuthenticateTestUser1();
        var bookSetUp = await _stayzaWebClient.PostAsJsonAsync("api/v1/books", new AddBookCommand(
            Title: Constants.Book.Title,
            Author: Constants.Book.Author,
            ISBN: Constants.Book.ISBN));
        
        // Act
        var bookResponse = await _stayzaWebClient.PostAsJsonAsync("api/v1/books", new AddBookCommand(
            Title: Constants.Book.Title,
            Author: Constants.Book.Author,
            ISBN: Constants.Book.ISBN));
        
        // Assert
        bookResponse.IsSuccessStatusCode.ShouldBeFalse();
    }
}