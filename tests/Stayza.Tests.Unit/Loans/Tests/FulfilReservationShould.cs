using Shouldly;
using Stayza.Core.Exceptions;
using Stayza.Domain.Loans.Events;
using Stayza.Tests.Unit.Utils;
using Stayza.Tests.Unit.Utils.TestConstants;

namespace Stayza.Tests.Unit.Loans.Tests;

public class FulfilReservationShould
{
    [Fact]
    public void Update_reservation_status_and_expiration_time()
    {
        // Arrange
        var bookCopy = TestBooksFactory.CreateBookCopy();
        var reservation = bookCopy.Reserve(
            userId: Constants.Users.Id,
            utcNow: Constants.Time.TestUtcNow);

        // Act
        bookCopy.FulfillReservation(
            reservationId: reservation.Id,
            utcNow: Constants.Time.TestUtcNow);

        // Assert
        bookCopy.ActiveReservation.ShouldNotBeNull();
        bookCopy.ActiveReservation.ExpiresAt.ShouldBe(Constants.Time.TestUtcNow.AddDays(3));
        bookCopy.ActiveReservation.UserId.ShouldBe(Constants.Users.Id);
        bookCopy.ActiveReservation.BookCopyId.ShouldBe(bookCopy.Id);
    }

    [Fact]
    public void Add_a_reservation_fulfilled_domain_event()
    {
        // Arrange
        var bookCopy = TestBooksFactory.CreateBookCopy();
        var reservation = bookCopy.Reserve(
            userId: Constants.Users.Id,
            utcNow: Constants.Time.TestUtcNow);

        // Act
        bookCopy.FulfillReservation(
            reservationId: reservation.Id,
            utcNow: Constants.Time.TestUtcNow);

        // Assert
        bookCopy.DomainEvents.Count(x =>
            x.GetType().Name == nameof(ReservationFulfilledEvent)).ShouldBe(1);
        (bookCopy.DomainEvents.First() as ReservationFulfilledEvent)!.UserId.ShouldBe(Constants.Users.Id);
        (bookCopy.DomainEvents.First() as ReservationFulfilledEvent)!.BookCopyId.ShouldBe(bookCopy.Id);
    }

    [Fact]
    public void Fail_if_reservation_is_cancelled()
    {
        // HW: add a test here.
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
            () => bookCopy.StartLoan(
                userId: Constants.Users.Id,
                utcNow: Constants.Time.TestUtcNow));
    }

    [Fact]
    public void Fail_if_reservation_does_not_exist()
    {
        // HW: add a test here.
        // Arrange
        var bookCopy = TestBooksFactory.CreateBookCopy();
        
        // Act & Assert
        Assert.Throws<EntityNotFoundException>(
            () => bookCopy.CancelReservation(
                reservationId: Constants.BookCopy.BookCopyId,
                reason: "EXPIRED"));
    }
}