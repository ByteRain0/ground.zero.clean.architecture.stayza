using System.Net.Http.Json;
using Shouldly;
using Stayza.Application.Books.Commands;
using Stayza.Core.Messaging;
using Stayza.Domain.Books;
using Stayza.Domain.Loans;
using Stayza.Infrastructure.Persistence.DataSeed;
using Stayza.Tests.Integration.Base;
using Stayza.Tests.Integration.Base.TestConstants;

namespace Stayza.Tests.Integration.Loans.Tests;

public class LoanBookCopyEndpointShould :
    IClassFixture<ApiFactory>,
    IAsyncLifetime
{
    private readonly HttpClient _stayzaWebClient;

    private readonly RabbitMqTestMessageConsumer _messageConsumer;
    
    private Func<Task> _resetDatabase;
    
    private Func<Task> _createDbSnapshot;

    private readonly string _testSpecificExchangeName;

    public LoanBookCopyEndpointShould(ApiFactory factory)
    {
        _stayzaWebClient = factory.CreateClient();
        _testSpecificExchangeName = factory.ExchangeName;
        _messageConsumer = factory.MessageConsumer;
        _resetDatabase = factory.ResetDatabaseAsync;
        _createDbSnapshot = factory.InitializeDbRespawner;
    }

    [Fact]
    public async Task Start_new_loan()
    {
        // Arrange
        using var rootActivity = OtelTestFramework.Source.StartActivity();
        
        using var arrangeActivity = OtelTestFramework.Source.StartActivity("Arrange phase");
        _stayzaWebClient.InjectTraceContext(arrangeActivity!);
        
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

        // Reserve a book before loaning
        await _stayzaWebClient.PostAsync($"api/v1/book-copies/{bookCopy!.Id}/reservations", default);

        arrangeActivity?.Stop();
        
        // Act
        using var actActivity = OtelTestFramework.Source.StartActivity("Act phase");
        _stayzaWebClient.InjectTraceContext(actActivity!);
        
        var consumeEvents = await _messageConsumer.BindAndConsumeAsync(
            exchangeName: _testSpecificExchangeName,
            routingKey: RoutingKeys
                .BookCopyLoanedTopic
                .ReplaceBookCopyIdPlaceholderWith("*"),
            timeout: TimeSpan.FromSeconds(60),
            testName: nameof(Start_new_loan));
        
        var loanResponse = await _stayzaWebClient.PostAsync($"api/v1/book-copies/{bookCopy.Id}/loans", default);

        actActivity?.Stop();
        
        // Assert
        loanResponse.IsSuccessStatusCode.ShouldBeTrue();
        var loan = await loanResponse.Content.ReadFromJsonAsync<Loan>();
        loan.BookCopyId.ShouldBe(bookCopy.Id);
        loan.UserId.ShouldBe(TestUserSeeder.TestUser1Id);
        loan.IsReturned.ShouldBeFalse();

        // Await and assert the background message result
        (await consumeEvents).ShouldBeTrue();
    }

    public Task InitializeAsync() => _createDbSnapshot();

    public Task DisposeAsync() => _resetDatabase();
}