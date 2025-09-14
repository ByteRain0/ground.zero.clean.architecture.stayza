using Shouldly;
using Stayza.Core.Exceptions;
using Stayza.Domain.Loans.Events;
using Stayza.Domain.Loans.Exceptions;
using Stayza.Tests.Unit.Utils;
using Stayza.Tests.Unit.Utils.TestConstants;

namespace Stayza.Tests.Unit.Loans.Tests;

public class StartLoanShould
{
    [Fact]
    public void Fail_if_book_copy_is_already_loaned()
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
                userId: Constants.Users.Id,
                utcNow: Constants.Time.TestUtcNow));
    }

    [Fact]
    public void Fail_if_user_does_not_have_a_reservation_for_book_copy()
    {
        // HW
        // Arrange
        var bookCopy = TestBooksFactory.CreateBookCopy();

        // Act / Assert
        Assert.Throws<EntityNotFoundException>(
            () => bookCopy.StartLoan(
                userId: Constants.Users.Id,
                utcNow: Constants.Time.TestUtcNow));
    }

    [Fact]
    public void Remove_existing_reservation()
    {
        // HW
        // Arrange
        var bookCopy = TestBooksFactory.CreateBookCopy();
        var reservation = bookCopy.Reserve(
            userId: Constants.Users.Id,
            utcNow: Constants.Time.TestUtcNow);

        // Act
        bookCopy.CancelReservation(
            reservationId: reservation.Id,
            reason: "TEST_REMOVAL");

        // Assert
        bookCopy.ActiveReservation.ShouldBeNull();
    }

    [Fact]
    public void Start_new_loan_and_publish_BookLoanedEvent()
    {
        // HW
        // Arrange
        var bookCopy = TestBooksFactory.CreateBookCopy();
        var reservation = bookCopy.Reserve(
            userId: Constants.Users.Id,
            utcNow: Constants.Time.TestUtcNow);

        // Act
        bookCopy.StartLoan(
            userId: Constants.Users.Id,
            utcNow: Constants.Time.TestUtcNow);

        // Assert
        bookCopy.CurrentLoan.ShouldNotBeNull();
        bookCopy.CurrentLoan.UserId.ShouldBe(Constants.Users.Id);
        bookCopy.CurrentLoan.BookCopyId.ShouldBe(bookCopy.Id);
        bookCopy.DomainEvents.Count(x =>
            x.GetType().Name == nameof(BookLoanedEvent)).ShouldBe(1);
    }
}