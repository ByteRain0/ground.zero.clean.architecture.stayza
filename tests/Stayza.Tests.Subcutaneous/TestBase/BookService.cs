using Microsoft.EntityFrameworkCore;
using Shouldly;
using Stayza.Application.Books;
using Stayza.Application.Books.Commands;
using Stayza.Infrastructure.Persistence;
using Stayza.Tests.Subcutaneous.Base.TestConstants;

namespace Stayza.Tests.Subcutaneous.TestBase;

public class BookServiceShould 
    : IClassFixture<TestApplicationFactory>, IAsyncLifetime
{
    private TestApplicationFactory _testFactory;
    private BooksService _sut;
    private ApplicationDbContext _dbContext;

    public BookServiceShould(TestApplicationFactory testFactory)
    {
        _testFactory = testFactory;
        _sut = _testFactory.GetService<BooksService>();
        _dbContext = _testFactory.GetService<ApplicationDbContext>();
    }
    
    [Fact]
    public async Task Create_new_book()
    {
        // Arrange

        // Act
        var book = await _sut.AddBook(new AddBookCommand(
            Title: Constants.Book.Title,
            Author: Constants.Book.Author,
            ISBN: Constants.Book.ISBN));

        // Assert
        book.Title.ShouldBe(Constants.Book.Title);
        book.Author.ShouldBe(Constants.Book.Author);
        book.ISBN.ShouldBe(Constants.Book.ISBN);
    }

    [Fact]
    public async Task Get_book_by_isbn()
    {
        // Arrange
        await _sut.AddBook(new AddBookCommand(
            Title: Constants.Book.Title,
            Author: Constants.Book.Author,
            ISBN: Constants.Book.ISBN));
        
        // Act
        var bookFromDb = await _sut.GetBookByIsbn(Constants.Book.ISBN, new CancellationToken());
        
        // Assert
        bookFromDb.Title.ShouldBe(Constants.Book.Title);
        bookFromDb.Author.ShouldBe(Constants.Book.Author);
        bookFromDb.ISBN.ShouldBe(Constants.Book.ISBN);
    }

    public async Task InitializeAsync()
    {
        await _dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        // Cleanup / Teardown
        await _dbContext.Books.ExecuteDeleteAsync();
    }
}