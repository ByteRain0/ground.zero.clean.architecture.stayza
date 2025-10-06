using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Snapshooter.Xunit;
using Stayza.Application.Books.Commands;
using Stayza.Domain.Books;
using Stayza.Tests.Core;
using Stayza.Tests.Integration.Base_V2;
using Stayza.Tests.Integration.Base;
using Stayza.Tests.Integration.Base.TestConstants;
using Stayza.Tests.Integration.Mocks;

namespace Stayza.Tests.Integration.Books.Tests;

public class AddBookEndpointV2Should : 
    IClassFixture<RabbitMqLessApplicationFactory>,
    IAsyncLifetime
{
    private readonly HttpClient _client;

    private Func<Task> _resetDatabase;

    private Func<Task> _createDbSnapshot;
    private readonly MockEventsStore _eventsStore;
    
    public AddBookEndpointV2Should(RabbitMqLessApplicationFactory webApplicationFactory)
    {
        _resetDatabase = webApplicationFactory.ResetDatabaseAsync;
        _createDbSnapshot = webApplicationFactory.InitializeDbRespawner; 
        _client = webApplicationFactory.CreateClient();
        _eventsStore = webApplicationFactory.Services.GetRequiredService<MockEventsStore>();
    }

    [Fact]
    public async Task Add_new_book()
    {
        // Arrange
        var newAddBookCommand = new AddBookCommand(
            Title: Constants.Book.Title,
            Author: Constants.Book.Author,
            ISBN: Constants.Book.ISBN);

        await _client.AuthenticateTestUser1();
        
        // Act
        var response = await _client.PostAsJsonAsync("api/v1/books", newAddBookCommand);
        
        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        var responseBook = await response.Content.ReadFromJsonAsync<Book>();
        Snapshot.Match<Book>(responseBook, options => options.IgnoreAllFields("Id"));
    }
    
    [Fact]
    public async Task Add_new_book_should_publish_domain_event()
    {
        // Arrange
        var newAddBookCommand = new AddBookCommand(
            Title: Constants.Book.Title,
            Author: Constants.Book.Author,
            ISBN: Constants.Book.ISBN);

        await _client.AuthenticateTestUser1();
        
        // Act
        await _client.PostAsJsonAsync("api/v1/books", newAddBookCommand);

        // Assert
        _eventsStore.GetEvent<NewBookAddedEvent>().RoutingKey.ShouldBe("book.added");
    }
    
    
    public async Task InitializeAsync()
    {
        await _createDbSnapshot();
    }

    public async Task DisposeAsync()
    {
        await _resetDatabase();
    }
}