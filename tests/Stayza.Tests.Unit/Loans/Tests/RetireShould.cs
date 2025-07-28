using Shouldly;
using Stayza.Domain.Loans.Events;
using Stayza.Tests.Unit.Utils;
using Stayza.Tests.Unit.Utils.TestConstants;

namespace Stayza.Tests.Unit.Loans.Tests;

public class RetireShould
{
    [Fact]
    public void Cancel_existing_active_reservations()
    {
        // Arrange
        var bookCopy = TestBooksFactory.CreateBookCopy();
        bookCopy.Reserve(
            userId: Constants.Users.Id,
            utcNow: Constants.Time.TestUtcNow);
        bookCopy.Reserve(
            userId: Constants.Users.Id2,
            utcNow: Constants.Time.TestUtcNow);
        
        // Act
        bookCopy.Retire();
        
        // Assert
        bookCopy.IsRetired.ShouldBeTrue();
        bookCopy.ActiveReservations.Count.ShouldBe(0);
        bookCopy.DomainEvents.Count(x => 
            x.GetType().Name == nameof(ReservationCancelledEvent)).ShouldBe(2);
    }
}