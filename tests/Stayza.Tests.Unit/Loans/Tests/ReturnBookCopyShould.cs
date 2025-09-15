using Shouldly;
using Stayza.Domain.Loans;
using Stayza.Domain.Loans.Events;
using Stayza.Tests.Unit.Utils;
using Stayza.Tests.Unit.Utils.TestConstants;

namespace Stayza.Tests.Unit.Loans.Tests;

public class ReturnBookCopyShould
{
    [Fact]
    public void Fail_if_copy_is_not_loaned()
    {
        // Arrange
        var bookCopy = TestBooksFactory.CreateBookCopy();
        
        // Act
        var action = () => bookCopy.Return(
            userId: Constants.Users.Id,
            utcNow: Constants.Time.TestUtcNow);

        //Assert
        var ex = Should.Throw<InvalidOperationException>(action);
        ex.Message.ShouldBe("Copy is not loaned.");
    }

    [Fact]
    public void Fail_if_book_copy_was_loaned_by_a_different_user()
    {
        // Arrange 
        var bookCopy = TestBooksFactory.CreateBookCopy();

        bookCopy.Reserve(
            userId: Constants.Users.Id,
            utcNow: Constants.Time.TestUtcNow);
        
        bookCopy.StartLoan(
            userId: Constants.Users.Id,
            utcNow: Constants.Time.TestUtcNow);
        
        // Act 
        var action = () => bookCopy.Return(
            userId: Constants.Users.Id2,
            utcNow: Constants.Time.TestUtcNow);
        
        // Assert
        var ex = Should.Throw<InvalidOperationException>(action);
        ex.Message.ShouldBe("Cannot return not owned book");
    }

    [Fact]
    public void Mark_current_loan_as_returned_and_publish_BookReturnedEvent()
    {
        // Assert
        var userId = Constants.Users.Id;
        var utcNow = Constants.Time.TestUtcNow;
        var bookCopy = TestBooksFactory.CreateBookCopy();

        bookCopy.Reserve(userId, utcNow);
        bookCopy.StartLoan(userId, utcNow);
        
        // Act 
        var loan = bookCopy.Return(
            userId: Constants.Users.Id,
            utcNow: Constants.Time.TestUtcNow);
        
        // Assert
        loan.ShouldBe(bookCopy.CurrentLoan);
        loan.IsReturned.ShouldBeTrue();
        loan.ReturnDate.ShouldBe(utcNow);

        bookCopy.DomainEvents.OfType<BookReturnedEvent>().Count().ShouldBe(1);
        
        var returnEvent = bookCopy.DomainEvents
            .OfType<BookReturnedEvent>()
            .Single();

        returnEvent.BookCopyId.ShouldBe(bookCopy.Id);
        returnEvent.LoanId.ShouldBe(loan.Id);
        returnEvent.ReturnDate.ShouldBe(utcNow);    }
}