using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;
using Shouldly;
using Stayza.Domain.Books;
using Stayza.Infrastructure.Persistence;
using Stayza.Infrastructure.Persistence.Interceptors;
using Stayza.Tests.Core;

namespace Stayza.Tests.EndToEnd;

[Collection("E2E")]
public class AddBookShould
{
    private readonly TestBase _testBase;
    
    private string CardGamePage => TestBase.WebAppUrl + "/books-admin";
    
    public AddBookShould(TestBase testBase)
    {
        _testBase = testBase;
    }

    [Fact]
    public async Task Add_new_book_if_not_exists()
    {
        // Arrange
        IPage page = await _testBase._browser.NewPageAsync();
        await page.GotoAsync(CardGamePage);
        var bookTitle = "Test E2E Book";
        var bookAuthor = "Jhon Doe";
        var bookIsbn = "1234566666";
        
        // Act
        
        await page.Locator("[name='Model.Title']").FillAsync(bookTitle);
        await page.Locator("[name='Model.Author']").FillAsync(bookAuthor);
        await page.Locator("[name='Model.ISBN']").FillAsync(bookIsbn);
        await page.Locator("button[type='submit']").ClickAsync();
        
        // Assert

        // Version 1: Assert that a specific message appeared
        var successMessageLocator = page.Locator("div.alert.alert-success");
        await successMessageLocator.WaitForAsync(new LocatorWaitForOptions {Timeout = 5000});

        // Version 2: Query the database
        await using var context = new ApplicationDbContext(_testBase.DatabaseOptions.Options, new PublishDomainEventsInterceptor(new MockProducer()));
        var bookFromDb = await context.Books.SingleAsync(x => x.ISBN == bookIsbn);
        bookFromDb.Author.ShouldBe(bookAuthor);
        bookFromDb.Title.ShouldBe(bookTitle);
        

        // Version 3: Query the API :)
        await _testBase.HttpClient.AuthenticateTestUser1();
        var bookResponse = await _testBase.HttpClient.GetAsync("api/v1/books/1234566666");
        var book = await bookResponse.Content.ReadFromJsonAsync<Book>();
        book.ISBN.ShouldBe(bookIsbn);
        book.Author.ShouldBe(bookAuthor);
        book.Title.ShouldBe(bookTitle);
    }
}