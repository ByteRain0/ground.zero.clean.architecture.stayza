using Shouldly;
using Stayza.Domain.Loans;
using Stayza.Domain.Loans.Exceptions;
using Stayza.Tests.Unit.Utils;
using Stayza.Tests.Unit.Utils.TestConstants;

namespace Stayza.Tests.Unit.Loans.Tests;

public class ReserveShould
{
    private readonly DateTimeOffset _testUtcNow = new(
        year: 2024,
        month: 12,
        day: 25,
        hour: 15,
        minute: 30,
        second: 0,
        offset: TimeSpan.Zero);

    [Fact]
    public void Reserve_book_when_book_is_available()
    {
        // Arrange
        var bookCopy = TestBooksFactory.CreateBookCopy();

        // Act
        var reservation = bookCopy.Reserve(
            userId: Constants.Users.Id,
            utcNow: _testUtcNow);

        // Assert
        reservation.ShouldNotBeNull();
        reservation.UserId.ShouldBe(Constants.Users.Id);
        reservation.BookCopyId.ShouldBe(Constants.BookCopy.BookCopyId);
        reservation.ReservedAt.ShouldBe(_testUtcNow);
        reservation.ExpiresAt.ShouldBe(_testUtcNow.AddDays(30));
        reservation.Status.ShouldBe(ReservationStatus.Active);
    }
    
    [Fact]
    public void Reserve_book_when_book_is_loaned()
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
        
        // Act 
        bookCopy.Reserve(
            userId: Constants.Users.Id2,
            utcNow: _testUtcNow);
        
        bookCopy.IsLoaned.ShouldBeTrue();
        bookCopy.ActiveReservations.Count.ShouldBe(1);
        bookCopy.ActiveReservations.First().UserId.ShouldBe(Constants.Users.Id2);
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
                utcNow: _testUtcNow));
    }

    [Fact]
    public void Fail_when_book_already_reserved_by_user()
    {
        // Arrange
        var bookCopy = TestBooksFactory.CreateBookCopy();
        bookCopy.Reserve(
            userId: Constants.Users.Id,
            utcNow: _testUtcNow);
        
        // Act / Assert
        Assert.Throws<ReservationAlreadyExistsException>(() =>
            bookCopy.Reserve(
                userId: Constants.Users.Id,
                utcNow: _testUtcNow));
    }
}