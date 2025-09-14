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
        // HW
        // Arrange
        var bookCopy = TestBooksFactory.CreateBookCopy();
        
        // Act & Assert
        Assert.Throws<EntityNotFoundException>(
            () => bookCopy.CancelReservation(
                reservationId: Constants.BookCopy.BookCopyId,
                reason: "EXPIRED"));
    }

    [Fact]
    public void Fail_if_reservation_already_cancelled()
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
            reason: "EXPIRED");

        // Assert
        Assert.Throws<EntityNotFoundException>(
            () => bookCopy.CancelReservation(
                reservationId: reservation.Id,
                reason: "EXPIRED"));
    }

    [Fact]
    public void Cancel_valid_reservation()
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
            reason: "EXPIRED");

        // Assert
        Assert.Equal(ReservationStatus.Cancelled, reservation.Status);
    }

    [Fact]
    public void Publish_ReservationCancelledEvent()
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
            reason: "EXPIRED");

        // Assert
        bookCopy.DomainEvents.Count(x =>
            x.GetType().Name == nameof(ReservationCancelledEvent)).ShouldBe(1);
        (bookCopy.DomainEvents.First() as ReservationCancelledEvent)!.UserId.ShouldBe(Constants.Users.Id);
        (bookCopy.DomainEvents.First() as ReservationCancelledEvent)!.BookCopyId.ShouldBe(bookCopy.Id);
    }
}