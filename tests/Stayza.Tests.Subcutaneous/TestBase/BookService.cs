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
    public async Task Get_book_by_id()
    {
        // Arrange
        var testFactory = new TestApplicationFactory();
        var sut = testFactory.GetService<BooksService>();
        
        var book = await sut.AddBook(new AddBookCommand(
            Title: Constants.Book.Title,
            Author: Constants.Book.Author,
            ISBN: Constants.Book.ISBN));
        
        // Act

        var getBook = await sut.GetBookById(book.Id, CancellationToken.None);
        
        // Assert
        getBook.Id.ShouldBe(book.Id);
        getBook.ISBN.ShouldBe(book.ISBN);
        getBook.Title.ShouldBe(book.Title);
        getBook.Author.ShouldBe(book.Author);
    }
    
    
    [Fact]
    public async Task Get_book_by_isbn()
    {
        // Arrange
        var testFactory = new TestApplicationFactory();
        var sut = testFactory.GetService<BooksService>();
        
        var book = await sut.AddBook(new AddBookCommand(
            Title: Constants.Book.Title,
            Author: Constants.Book.Author,
            ISBN: Constants.Book.ISBN));
        
        // Act

        var getBook = await sut.GetBookByIsbn(book.ISBN, CancellationToken.None);
        
        // Assert
        getBook.Id.ShouldBe(book.Id);
        getBook.ISBN.ShouldBe(book.ISBN);
        getBook.Title.ShouldBe(book.Title);
        getBook.Author.ShouldBe(book.Author);
    }
}