using System.Net.Http.Json;
using Shouldly;
using Stayza.Application.Books.Commands;
using Stayza.Domain.Books;
using Stayza.Domain.Loans;
using Stayza.Infrastructure.Persistence.DataSeed;
using Stayza.Tests.Integration.Base;
using Stayza.Tests.Integration.Base.TestConstants;

namespace Stayza.Tests.Integration.Loans.Tests;

//[Collection("IntegrationTests")]
public class ReturningBookShould : 
    IClassFixture<ApiFactory>,
    IAsyncLifetime
{
    private readonly HttpClient _stayzaWebClient;
    
    private Func<Task> _resetDatabase;

    private NotificationsApiServer _notificationsApiServer;
    
    public ReturningBookShould(ApiFactory factory)
    {
        _stayzaWebClient = factory.HttpClient;
        _resetDatabase = factory.ResetDatabaseAsync;
        _notificationsApiServer = factory.NotificationsApi;
    }
    
    // Example of test that run integration with external api.
    // This is run as a form of black box testing where we test from external user flow.
    [Fact]
    public async Task Notify_next_user_with_reservation_about_fulfillment()
    {
        // Arrange
        using var rootActivity = OtelTestFramework.Source.StartActivity();
        _stayzaWebClient.InjectTraceContext(rootActivity);
        
        await _notificationsApiServer.SetUpNotificationResponse(true);
        await _stayzaWebClient.AuthenticateTestUser1();

        // Set up a test book
        var bookResponse = await _stayzaWebClient.PostAsJsonAsync("api/v1/books", new AddBookCommand(
            Title: Constants.Book.Title,
            Author: Constants.Book.Author,
            ISBN: Constants.Book.ISBN));
        var book = await bookResponse.Content.ReadFromJsonAsync<Book>();

        // Set up a book copy
        var bookCopyResponse = await _stayzaWebClient.PostAsync($"api/v1/books/{book!.Id}/copies", default);
        var bookCopy = await bookCopyResponse.Content.ReadFromJsonAsync<BookCopy>();

        // Reserve a book before loaning for user 1
        await _stayzaWebClient.PostAsync($"api/v1/book-copies/{bookCopy!.Id}/reservations", default);

        // Reserve the book before loaning for user 2
        await _stayzaWebClient.AuthenticateTestUser2();
        await _stayzaWebClient.PostAsync($"api/v1/book-copies/{bookCopy!.Id}/reservations", default);
        
        // Loan the book from user 1 perspective
        await _stayzaWebClient.AuthenticateTestUser1();
        await _stayzaWebClient.PostAsync($"api/v1/book-copies/{bookCopy.Id}/loans", default);
        
        // Act
        var returnResponse = await _stayzaWebClient.PutAsync($"api/v1/book-copies/{bookCopy.Id}/loans/return", default);
        
        // Assert
        returnResponse.IsSuccessStatusCode.ShouldBeTrue();
        var loan = await returnResponse.Content.ReadFromJsonAsync<Loan>();
        loan.BookCopyId.ShouldBe(bookCopy.Id);
        loan.UserId.ShouldBe(TestUserSeeder.TestUser1Id);
        loan.IsReturned.ShouldBeTrue();
        
        await Task.Delay(TimeSpan.FromSeconds(10)); // Add about 10 seconds delay for the call to the notifications api to be made.
        (await _notificationsApiServer.CheckThatNotificationHasBeenReceived(
                userId: TestUserSeeder.TestUser2Id,
                notificationType:"ReservationFulfilled"))
            .ShouldBeTrue();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();
}