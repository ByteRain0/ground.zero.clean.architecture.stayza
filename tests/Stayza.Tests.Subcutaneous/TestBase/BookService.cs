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
    private readonly List<Guid> _booksCreatedDuringTest = new();

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
        _booksCreatedDuringTest.Add(book.Id);

        // Assert
        book.Title.ShouldBe(Constants.Book.Title);
        book.Author.ShouldBe(Constants.Book.Author);
        book.ISBN.ShouldBe(Constants.Book.ISBN);
    }

    [Fact]
    public async Task Get_book_by_isbn()
    {
        // Arrange
        var book = await _sut.AddBook(new AddBookCommand(
            Title: Constants.Book.Title,
            Author: Constants.Book.Author,
            ISBN: Constants.Book.ISBN));
        _booksCreatedDuringTest.Add(book.Id);

        // Act
        var bookFromDb = await _sut.GetBookByIsbn(Constants.Book.ISBN, new CancellationToken());
        
        // Assert
        bookFromDb.Title.ShouldBe(Constants.Book.Title);
        bookFromDb.Author.ShouldBe(Constants.Book.Author);
        bookFromDb.ISBN.ShouldBe(Constants.Book.ISBN);
    }

    [Fact]
    public async Task Get_book_by_id()
    {
        // Arrange
        var book = await _sut.AddBook(new AddBookCommand(
            Title: Constants.Book.Title,
            Author: Constants.Book.Author,
            ISBN: Constants.Book.ISBN));
        _booksCreatedDuringTest.Add(book.Id);

        // Act
        var bookFromDb = await _sut.GetBookById(book.Id, new CancellationToken());

        // Assert
        bookFromDb.Title.ShouldBe(Constants.Book.Title);
        bookFromDb.Author.ShouldBe(Constants.Book.Author);
        bookFromDb.ISBN.ShouldBe(Constants.Book.ISBN);
    }

    [Fact]
    public async Task Retire_book()
    {
        // Arrange
        var book = await _sut.AddBook(new AddBookCommand(
            Title: Constants.Book.Title,
            Author: Constants.Book.Author,
            ISBN: Constants.Book.ISBN));
        _booksCreatedDuringTest.Add(book.Id);

        var bookCopy = book.AddCopy(Constants.BookCopy.BookCopyId);

        // Act
        var bookFromDb = await _sut.Retire(new RetireBookCommand(BookId: book.Id));

        // Assert
        bookCopy.IsRetired.ShouldBeTrue();
    }

    [Fact]
    public async Task Remove_book_copy()
    {
        // Arrange
        var book = await _sut.AddBook(new AddBookCommand(
            Title: Constants.Book.Title,
            Author: Constants.Book.Author,
            ISBN: Constants.Book.ISBN));
        _booksCreatedDuringTest.Add(book.Id);

        var bookCopy = book.AddCopy(Constants.BookCopy.BookCopyId);

        // Act
        var bookFromDb = await _sut.RemoveBookCopy(new RemoveBookCopyCommand(BookId: book.Id, BookCopyId: bookCopy.Id));

        // Assert
        bookFromDb.ShouldNotBeNull();
        bookFromDb.Copies.ShouldNotContain(bookCopy);
    }

    [Fact]
    public async Task Add_book_copy()
    {
        // Arrange
        var book = await _sut.AddBook(new AddBookCommand(
            Title: Constants.Book.Title,
            Author: Constants.Book.Author,
            ISBN: Constants.Book.ISBN));
        _booksCreatedDuringTest.Add(book.Id);

        // Act
        var bookCopyFromDb = await _sut.AddBookCopy(new AddBookCopyCommand(BookId: book.Id));

        // Assert
        bookCopyFromDb.ShouldNotBeNull();
    }

    public async Task InitializeAsync()
    {
        await _dbContext.Database.EnsureCreatedAsync();
        _booksCreatedDuringTest.Clear();
    }

    public async Task DisposeAsync()
    {
        // Cleanup / Teardown
        if (_booksCreatedDuringTest.Any())
        {
            var booksToDelete = await _dbContext.Books
                .Where(b => _booksCreatedDuringTest.Contains(b.Id))
                .ToListAsync();

            _dbContext.Books.RemoveRange(booksToDelete);
            await _dbContext.SaveChangesAsync();
        }
    }
}