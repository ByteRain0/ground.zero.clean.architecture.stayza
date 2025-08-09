using Shouldly;
using Stayza.Core.Exceptions;
using Stayza.Domain.Loans;
using Stayza.Domain.Loans.Exceptions;
using Stayza.Tests.Unit.Utils;
using Stayza.Tests.Unit.Utils.TestConstants;

namespace Stayza.Tests.Unit.Loans.Tests;

public class ReserveShould
{
    [Fact]
    public void Reserve_book_when_book_is_available()
    {
        // Arrange
        var bookCopy = TestBooksFactory.CreateBookCopy();

        // Act
        var reservation = bookCopy.Reserve(
            userId: Constants.Users.Id,
            utcNow: Constants.Time.TestUtcNow);

        // Assert
        reservation.ShouldNotBeNull();
        reservation.UserId.ShouldBe(Constants.Users.Id);
        reservation.BookCopyId.ShouldBe(Constants.BookCopy.BookCopyId);
        reservation.ReservedAt.ShouldBe(Constants.Time.TestUtcNow);
        reservation.ExpiresAt.ShouldBe(Constants.Time.TestUtcNow.AddDays(30));
        reservation.Status.ShouldBe(ReservationStatus.Pending);
    }
    
    [Fact]
    public void Reserve_book_when_book_is_loaned()
    {
        // Arrange
        var book = TestBooksFactory.CreateBook();
        var bookCopy = book.AddCopy(Constants.BookCopy.BookCopyId);
        bookCopy.Reserve(
            userId: Constants.Users.Id,
            utcNow: Constants.Time.TestUtcNow);
        bookCopy.StartLoan(
            userId: Constants.Users.Id,
            utcNow: Constants.Time.TestUtcNow);
        
        // Act 
        bookCopy.Reserve(
            userId: Constants.Users.Id2,
            utcNow: Constants.Time.TestUtcNow);
        
        bookCopy.IsLoaned.ShouldBeTrue();
        bookCopy.PendingReservations.Count.ShouldBe(1);
        bookCopy.PendingReservations.First().UserId.ShouldBe(Constants.Users.Id2);
    }

    [Fact]
    public void Fail_when_book_is_retired()
    {
        // Arrange
        var bookCopy = TestBooksFactory.CreateBookCopy();
        bookCopy.Retire();

        // Act / Assert
        Assert.Throws<BookNotAvailableForReservation>(() =>
            bookCopy.Reserve(
                userId: Constants.Users.Id,
                utcNow: Constants.Time.TestUtcNow));
    }

    [Fact]
    public void Fail_when_book_already_reserved_by_user()
    {
        // Arrange
        var bookCopy = TestBooksFactory.CreateBookCopy();
        bookCopy.Reserve(
            userId: Constants.Users.Id,
            utcNow: Constants.Time.TestUtcNow);
        
        // Act / Assert
        Assert.Throws<EntityAlreadyExistsException>(() =>
            bookCopy.Reserve(
                userId: Constants.Users.Id,
                utcNow: Constants.Time.TestUtcNow));
    }
}