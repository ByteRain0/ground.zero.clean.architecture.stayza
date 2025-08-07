using System.Net.Http.Json;
using Shouldly;
using Stayza.Application.Books.Commands;
using Stayza.Domain.Books;
using Stayza.Domain.Loans;
using Stayza.Tests.Integration.Base;
using Stayza.Tests.Integration.Base.TestConstants;
using Stayza.Web.Infrastructure.Seed;

namespace Stayza.Tests.Integration.Loans.Tests;

public class LoanBookCopyEndpointShould : IClassFixture<ApiFactory>
{
    private readonly HttpClient _stayzaWebClient;
    
    public LoanBookCopyEndpointShould(ApiFactory factory)
    {
        _stayzaWebClient = factory.GetEnrichedApiClient();
    }

    [Fact]
    public async Task Start_new_loan()
    {
        // Arrange
        await _stayzaWebClient.AuthenticateTestUser();
        
        // Set up a test book
        var bookResponse = await _stayzaWebClient.PostAsJsonAsync("api/v1/books", new AddBookCommand(
            Title:Constants.Book.Title,
            Author:Constants.Book.Author,
            ISBN:Constants.Book.ISBN));
        var book = await bookResponse.Content.ReadFromJsonAsync<Book>();

        // Set up a book copy
        var bookCopyResponse = await _stayzaWebClient.PostAsync($"api/v1/books/{book!.Id}/copies", default);
        var bookCopy = await bookCopyResponse.Content.ReadFromJsonAsync<BookCopy>();

        // Reserve a book before loaning
        await _stayzaWebClient.PostAsync($"api/v1/book-copies/{bookCopy!.Id}/reservations", default);
        
        // Act
        var loanResponse = await _stayzaWebClient.PostAsync($"api/v1/book-copies/{bookCopy.Id}/loans", default);
        
        // Assert
        loanResponse.IsSuccessStatusCode.ShouldBeTrue();
        var loan = await loanResponse.Content.ReadFromJsonAsync<Loan>();
        loan.BookCopyId.ShouldBe(bookCopy.Id);
        loan.UserId.ShouldBe(TestUserSeeder.TestUserId);
        loan.IsReturned.ShouldBeFalse();
    }
}