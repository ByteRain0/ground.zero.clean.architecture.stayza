using Shouldly;
using Stayza.Core.Exceptions;
using Stayza.Domain.Loans;
using Stayza.Domain.Loans.Events;
using Stayza.Tests.Unit.Utils;
using Stayza.Tests.Unit.Utils.TestConstants;

namespace Stayza.Tests.Unit.Loans.Tests;

public class CancelReservationShould
{
    [Fact]
    public void Fail_if_reservation_does_not_exist()
    {
        // Arrange
        var bookCopy = TestBooksFactory.CreateBookCopy();

        // Act 
        var action = () => bookCopy.CancelReservation(
            reservationId: Guid.NewGuid(),
            "cancel");

        // Assert
        var ex = Should.Throw<EntityNotFoundException>(action);
        ex.EntityType.ShouldBe(nameof(Reservation));
    }

    [Fact]
    public void Fail_if_reservation_already_cancelled()
    {
        // Arrange
        var bookCopy = TestBooksFactory.CreateBookCopy();

        var reservation = bookCopy.Reserve(
            userId: Constants.Users.Id,
            utcNow: Constants.Time.TestUtcNow);
        
        var action = () => bookCopy.CancelReservation(
            reservationId: reservation.Id,
            "cancel");

        action.Invoke();

        // Assert & Act
        var ex = Should.Throw<EntityNotFoundException>(action);
        ex.EntityType.ShouldBe(nameof(Reservation));
    }

    [Fact]
    public void Cancel_valid_reservation()
    {
        // Arrange
        var bookCopy = TestBooksFactory.CreateBookCopy();

        var reservation = bookCopy.Reserve(
            userId: Constants.Users.Id,
            utcNow: Constants.Time.TestUtcNow);
        
        // Act
        bookCopy.CancelReservation(
            reservationId: reservation.Id,
            "cancel");

        // Assert
        reservation.Status.ShouldBe(ReservationStatus.Cancelled);
    }

    [Fact]
    public void Publish_ReservationCancelledEvent()
    {
        // Arrange
        var bookCopy = TestBooksFactory.CreateBookCopy();
        const string cancelReason = "cancel";

        var reservation = bookCopy.Reserve(
            userId: Constants.Users.Id,
            utcNow: Constants.Time.TestUtcNow);
        
        // Act
        bookCopy.CancelReservation(
            reservationId: reservation.Id,
            reason: cancelReason);

        // Assert
        bookCopy.DomainEvents.OfType<ReservationCancelledEvent>().Count().ShouldBe(1);

        var @event = bookCopy.DomainEvents.OfType<ReservationCancelledEvent>().Single();
        
        @event.ReservationId.ShouldBe(reservation.Id);
        @event.UserId.ShouldBe(reservation.UserId);
        @event.BookCopyId.ShouldBe(bookCopy.Id);
        @event.Reason.ShouldBe(cancelReason);
    }
}