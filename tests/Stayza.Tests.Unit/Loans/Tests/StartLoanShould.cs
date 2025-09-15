using Shouldly;
using Stayza.Core.Exceptions;
using Stayza.Domain.Loans;
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
        // Arrange
        var bookCopy = TestBooksFactory.CreateBookCopy();

        bookCopy.Reserve(
            userId: Constants.Users.Id,
            utcNow: Constants.Time.TestUtcNow);
        
        var action = () => bookCopy.StartLoan(
            userId: Constants.Users.Id,
            utcNow: Constants.Time.TestUtcNow);

        action.Invoke();
        
        // Act & Assert
        Should.Throw<BookCopyAlreadyLoanedException>(action);
    }

    [Fact]
    public void Fail_if_user_does_not_have_a_reservation_for_book_copy()
    {
        // Arrange
        var bookCopy = TestBooksFactory.CreateBookCopy();
        
        bookCopy.Reserve(
            userId: Constants.Users.Id,
            utcNow: Constants.Time.TestUtcNow);
        
        // Act
        var action = () => bookCopy.StartLoan(
            userId: Constants.Users.Id2,
            utcNow: Constants.Time.TestUtcNow);

        // Assert
        var ex = Should.Throw<EntityNotFoundException>(action);
        ex.EntityType.ShouldBe(nameof(Reservation));
    }

    [Fact]
    public void Remove_existing_reservation()
    {
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
        bookCopy.Reservations.Contains(reservation).ShouldBeFalse();
    }

    [Fact]
    public void Start_new_loan_and_publish_BookLoanedEvent()
    {
        // Arrange
        var bookCopy = TestBooksFactory.CreateBookCopy();
        
        var reservation = bookCopy.Reserve(
            userId: Constants.Users.Id,
            utcNow: Constants.Time.TestUtcNow);
        
        // Act
        var loan = bookCopy.StartLoan(
            userId: Constants.Users.Id,
            utcNow: Constants.Time.TestUtcNow);

        // Assert
        bookCopy.DomainEvents.OfType<BookLoanedEvent>().Count().ShouldBe(1);
        
        var @event = bookCopy.DomainEvents
            .OfType<BookLoanedEvent>()
            .Single();
        
        @event.BookCopyId.ShouldBe(bookCopy.Id);
        @event.UserId.ShouldBe(reservation.UserId);
        @event.LoanId.ShouldBe(loan.Id);
        @event.LoanDate.ShouldBe(loan.TimeRange.Start);
        @event.DueDate.ShouldBe(loan.TimeRange.End);
    }
}