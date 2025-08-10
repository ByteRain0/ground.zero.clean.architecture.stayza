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

public class RemoveExpiredReservationsShould : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _apiFactory;

    public RemoveExpiredReservationsShould(ApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
    }
    
    //HW: add an invariant test that would ensure that:
    // - cancelled reservations are also deleted
    // - reservations that are not expired are un-affected.
    [Fact]
    public async Task Remove_expired_reservations()
    {
        // Arrange
        using var servicesScope = _apiFactory.Services.CreateScope();
        var dbContext = servicesScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var timeProvider = servicesScope.ServiceProvider.GetRequiredService<TimeProvider>();
        var sut = servicesScope.ServiceProvider.GetRequiredService<LoansBackgroundJobs>();
        
        var book = new Book(
            title: Constants.Book.Title,
            author: Constants.Book.Author,
            isbn: Constants.Book.ISBN,
            id: Constants.Book.Id);
        book.AddCopy(Constants.BookCopy.BookCopyId);
        
        await dbContext.Books.AddAsync(book);
        await dbContext.Reservations.AddAsync(new Reservation(
            userId: TestUserSeeder.TestUser1Id,
            reservedAt: timeProvider.GetUtcNow().AddDays(-60),
            bookCopyId: Constants.BookCopy.BookCopyId,
            id: Guid.NewGuid()));

        await dbContext.SaveChangesAsync();
        
        // Act
        await sut.RemoveExpiredAndCancelledReservations();
        
        // Assert
        var remainingReservations = await dbContext.Reservations.ToListAsync();
        remainingReservations.Count.ShouldBe(0);
    }
}