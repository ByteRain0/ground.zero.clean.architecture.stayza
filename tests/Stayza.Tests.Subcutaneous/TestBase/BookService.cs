using Shouldly;
using Stayza.Application.Books;
using Stayza.Application.Books.Commands;
using Stayza.Tests.Subcutaneous.Base.TestConstants;

namespace Stayza.Tests.Subcutaneous.TestBase;

public class BookServiceShould
{
    [Fact]
    public async Task Create_new_book()
    {
        // Arrange
        var testFactory = new TestApplicationFactory();
        var sut = testFactory.GetService<BooksService>();

        // Act
        var book = await sut.AddBook(new AddBookCommand(
            Title: Constants.Book.Title,
            Author: Constants.Book.Author,
            ISBN: Constants.Book.ISBN));

        // Assert
        book.Title.ShouldBe(Constants.Book.Title);
        book.Author.ShouldBe(Constants.Book.Author);
        book.ISBN.ShouldBe(Constants.Book.ISBN);
    }
}