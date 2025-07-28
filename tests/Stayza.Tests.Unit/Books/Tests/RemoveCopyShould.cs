using Shouldly;
using Stayza.Core.Exceptions;
using Stayza.Tests.Unit.Utils;
using Stayza.Tests.Unit.Utils.TestConstants;

namespace Stayza.Tests.Unit.Books.Tests;

public class RemoveCopyShould
{
    private readonly DateTimeOffset _testUtcNow = new DateTimeOffset(
        year: 2024,
        month: 12,
        day: 25,
        hour: 15,
        minute: 30,
        second: 0,
        offset: TimeSpan.Zero);

    [Fact]
    public void Remove_book_when_copy_is_not_loaned()
    {
        // Arrange
        var book = TestBooksFactory.CreateBook();
        book.AddCopy(Constants.BookCopy.BookCopyId);
        
        // Act
        book.RemoveCopy(Constants.BookCopy.BookCopyId);
        
        // Assert
        book.Copies.Count.ShouldBe(0);
    }
    
    [Fact]
    public void Fail_when_copy_is_loaned()
    {
        // Arrange
        var book = TestBooksFactory.CreateBook();
        var bookCopy = book.AddCopy(Constants.BookCopy.BookCopyId);
        bookCopy.Reserve(
            userId: Constants.Users.Id,
            utcNow: _testUtcNow);
        bookCopy.StartLoan(
            userId: Constants.Users.Id,
            utcNow: _testUtcNow);

        // Act / Assert
        Assert.Throws<InvalidOperationException>(
            () => book.RemoveCopy(Constants.BookCopy.BookCopyId));
    }

    [Fact]
    public void Fail_when_copy_not_found()
    {
        // Arrange
        var book = TestBooksFactory.CreateBook();
        
        // Act / Assert
        Assert.Throws<EntityNotFoundException>(
            () => book.RemoveCopy(Constants.BookCopy.BookCopyId));
    }
}