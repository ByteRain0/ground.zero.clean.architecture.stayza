using Shouldly;
using Stayza.Tests.Unit.Utils;
using Stayza.Tests.Unit.Utils.TestConstants;

namespace Stayza.Tests.Unit.Books.Tests;

public class AddCopyShould
{
    [Fact]
    public void Add_new_copy_when_valid_id()
    {
        // Arrange
        var book = TestBooksFactory.CreateBook();
        
        // Act
        book.AddCopy(Constants.BookCopy.BookCopyId);

        // Assert
        book.Copies.Count.ShouldBe(1);
        book.Copies.Count(x => x.Id == Constants.BookCopy.BookCopyId).ShouldBe(1);
    }

    [Fact]
    public void Fail_when_copy_already_exists()
    {
        // Arrange
        var book = TestBooksFactory.CreateBook();
        book.AddCopy(Constants.BookCopy.BookCopyId);
        
        // Act
        Assert.Throws<InvalidOperationException>(() => book.AddCopy(Constants.BookCopy.BookCopyId));
    }
}