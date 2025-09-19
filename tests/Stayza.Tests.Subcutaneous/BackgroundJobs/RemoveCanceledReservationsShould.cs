using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Stayza.Application.Loans;
using Stayza.Domain.Books;
using Stayza.Domain.Loans;
using Stayza.Infrastructure.Persistence;
using Stayza.Infrastructure.Persistence.DataSeed;
using Stayza.Tests.Subcutaneous.Base;
using Stayza.Tests.Subcutaneous.Base.TestConstants;

namespace Stayza.Tests.Subcutaneous.BackgroundJobs;

public class RemoveCanceledReservationsShould(ApiFactory apiFactory) : IClassFixture<ApiFactory>
{
    // HW: add subcutaneous tests in here.
    // - cancelled reservations are also deleted

    [Fact]
    public async Task Remove_canceled_reservation()
    {
        // Arrange
        using var servicesScope = apiFactory.Services.CreateScope();

        var repository = servicesScope.ServiceProvider.GetRequiredService<IBooksRepository>();
        var timeProvider = servicesScope.ServiceProvider.GetRequiredService<TimeProvider>();
        
        var book = new Book(
            title: Constants.Book.Title,
            author: Constants.Book.Author,
            isbn: Constants.Book.ISBN,
            id: Constants.Book.Id);
        
        var copy = book.AddCopy(Constants.BookCopy.BookCopyId);
        
        var reservation1 = copy.Reserve(
            userId: TestUserSeeder.TestUser1Id,
            utcNow: timeProvider.GetUtcNow().AddDays(-60));
        
        var reservation2 = copy.Reserve(
            userId: TestUserSeeder.TestUser2Id,
            utcNow: timeProvider.GetUtcNow().AddDays(1));
        
        await repository.AddBook(book);
        
        var sut = servicesScope.ServiceProvider.GetRequiredService<LoansBackgroundJobs>();
        await sut.CancelExpiredReservations();
        
        // Act
        await sut.RemoveCancelledReservations();
        
        // Assert
        var dbContext = servicesScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var remainingReservations = await dbContext.Reservations.ToListAsync();

        remainingReservations.ShouldNotContain(x => x.Status == ReservationStatus.Cancelled);
        
        remainingReservations.First(x => x.Id == reservation2.Id)
            .Status.ShouldNotBe(ReservationStatus.Cancelled);
    }
}