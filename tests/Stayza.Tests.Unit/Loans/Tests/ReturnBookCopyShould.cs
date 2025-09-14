using Shouldly;
using Stayza.Domain.Loans.Events;
using Stayza.Domain.Loans.Exceptions;
using Stayza.Tests.Unit.Utils;
using Stayza.Tests.Unit.Utils.TestConstants;

namespace Stayza.Tests.Unit.Loans.Tests;

public class ReturnBookCopyShould
{
    [Fact]
    public void Fail_if_copy_is_not_loaned()
    {
        // HW
        // Arrange
        var bookCopy = TestBooksFactory.CreateBookCopy();

        // Act / Assert
        Assert.Throws<InvalidOperationException>(
            () => bookCopy.Return(
                userId: Constants.Users.Id,
                utcNow: Constants.Time.TestUtcNow));
    }

    [Fact]
    public void Fail_if_book_copy_was_loaned_by_a_different_user()
    {
        // HW
        // Arrange
        var bookCopy = TestBooksFactory.CreateBookCopy();
        bookCopy.Reserve(
            userId: Constants.Users.Id,
            utcNow: Constants.Time.TestUtcNow);
        bookCopy.StartLoan(
            userId: Constants.Users.Id,
            utcNow: Constants.Time.TestUtcNow);

        // Act / Assert
        Assert.Throws<BookCopyAlreadyLoanedException>(
            () => bookCopy.StartLoan(
                userId: Constants.Users.Id2,
                utcNow: Constants.Time.TestUtcNow));
    }

    [Fact]
    public void Mark_current_loan_as_returned_and_publish_BookReturnedEvent()
    {
        // HW
        // Arrange
        var bookCopy = TestBooksFactory.CreateBookCopy();
        bookCopy.Reserve(
            userId: Constants.Users.Id,
            utcNow: Constants.Time.TestUtcNow);
        bookCopy.StartLoan(
            userId: Constants.Users.Id,
            utcNow: Constants.Time.TestUtcNow);

        // Act
        bookCopy.Return(
            userId: Constants.Users.Id,
            utcNow: Constants.Time.TestUtcNow);

        // Assert
        var bookReturnedEvents = bookCopy.DomainEvents
            .Where(x => x.GetType().Name == nameof(BookReturnedEvent))
            .ToList();

        bookReturnedEvents.Count.ShouldBe(1);
        (bookReturnedEvents.First() as BookReturnedEvent)!.BookCopyId.ShouldBe(bookCopy.Id);
        (bookReturnedEvents.First() as BookReturnedEvent)!.ReturnDate.ShouldBe(Constants.Time.TestUtcNow);
    }
}