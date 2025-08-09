using System.Net;
using System.Net.Http.Json;
using Shouldly;
using Stayza.Application.Books.Commands;
using Stayza.Domain.Books;
using Stayza.Tests.Integration.Base;
using Stayza.Tests.Integration.Base.TestConstants;

namespace Stayza.Tests.Integration.Books.Tests;

public class AddBookEndpointShould : IClassFixture<ApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _stayzaWebClient;

    private Func<Task> _resetDatabaseCommand;

    public AddBookEndpointShould(ApiFactory apiFactory)
    {
        _stayzaWebClient = apiFactory.HttpClient;
        _resetDatabaseCommand = apiFactory.ResetDatabaseAsync;
    }

    [Fact]
    public async Task Add_new_book()
    {
        // Arrange
        await _stayzaWebClient.AuthenticateTestUser();

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


    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabaseCommand();
}