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

public class CancelExpiredReservationsShould : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _apiFactory;

    public CancelExpiredReservationsShould(ApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
    }
    
    //HW: add an invariant test that would ensure that:
    // - cancelled reservations are also deleted
    // - reservations that are not expired are un-affected.
    [Fact]
    public async Task Cancel_expired_reservations()
    {
        // Arrange
        using var servicesScope = _apiFactory.Services.CreateScope();

        var repository = servicesScope.ServiceProvider.GetRequiredService<IBooksRepository>();
        var timeProvider = servicesScope.ServiceProvider.GetRequiredService<TimeProvider>();
        
        var book = new Book(
            title: Constants.Book.Title,
            author: Constants.Book.Author,
            isbn: Constants.Book.ISBN,
            id: Constants.Book.Id);
        var copy = book.AddCopy(Constants.BookCopy.BookCopyId);
        copy.Reserve(
            userId: TestUserSeeder.TestUser1Id,
            utcNow: timeProvider.GetUtcNow().AddDays(-60));
        
        await repository.AddBook(book);
        var sut = servicesScope.ServiceProvider.GetRequiredService<LoansBackgroundJobs>();
        
        // Act
        await sut.CancelExpiredReservations();
        
        // Assert
        var dbContext = servicesScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var remainingReservations = await dbContext.Reservations.ToListAsync();
        
        remainingReservations.First().Status.ShouldBe(ReservationStatus.Cancelled);
    }
}