using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Stayza.Application.Books;
using Stayza.Application.Books.Commands;
using Stayza.Application.Loans;
using Stayza.Infrastructure.Persistence;
using Stayza.Infrastructure.Persistence.DataSeed;
using Stayza.Tests.Subcutaneous.Base;
using Stayza.Tests.Subcutaneous.Base.TestConstants;

namespace Stayza.Tests.Subcutaneous.BackgroundJobs;

public class RemoveCanceledReservationsShould : IClassFixture<ApiFactory>, IAsyncLifetime
{
    private readonly ApiFactory _apiFactory;
    private IServiceScope _servicesScope;
    private BooksService _booksService;
    private TimeProvider _timeProvider;
    private ApplicationDbContext _dbContext;
    private LoansBackgroundJobs _sut;

    private readonly List<Guid> _bookIdsCreated = new();
    private readonly List<Guid> _bookCopyIdsCreated = new();
    private readonly List<Guid> _reservationIdsCreated = new();

    public RemoveCanceledReservationsShould(ApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _servicesScope = _apiFactory.Services.CreateScope();
        _dbContext = _servicesScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _booksService = _servicesScope.ServiceProvider.GetRequiredService<BooksService>();
        _timeProvider = _servicesScope.ServiceProvider.GetRequiredService<TimeProvider>();
        _sut = _servicesScope.ServiceProvider.GetRequiredService<LoansBackgroundJobs>();
    }

    // HW: add subcutaneous tests in here.
    [Fact]
    public async Task Remove_cancelled_reservations()
    {
        // Arrange
        var book = await _booksService.AddBook(new AddBookCommand(
            Title: Constants.Book.Title,
            Author: Constants.Book.Author,
            ISBN: Constants.Book.ISBN));
        var copy = book.AddCopy(book.Id);
        var reservation = copy.Reserve(
            userId: TestUserSeeder.TestUser1Id,
            utcNow: _timeProvider.GetUtcNow().AddDays(10));

        copy.CancelReservation(reservation.Id, "book_copy is cancelled");

        _bookIdsCreated.Add(book.Id);
        _bookCopyIdsCreated.Add(copy.Id);
        _reservationIdsCreated.Add(reservation.Id);

        // Act
        await _sut.RemoveCancelledReservations();

        // Assert
        var cancelledReservation = _dbContext.Reservations.Find(reservation.Id);
        cancelledReservation.ShouldBeNull();
    }

    public Task InitializeAsync()
    {
        _bookIdsCreated.Clear();
        _bookCopyIdsCreated.Clear();
        _reservationIdsCreated.Clear();
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        if (_reservationIdsCreated.Any())
        {
            var reservationsToDelete = await _dbContext.Reservations.Where(r => _reservationIdsCreated.Contains(r.Id)).ToListAsync();
            _dbContext.Reservations.RemoveRange(reservationsToDelete);
        }
        if (_bookCopyIdsCreated.Any())
        {
            var copiesToDelete = await _dbContext.BookCopies.Where(bc => _bookCopyIdsCreated.Contains(bc.Id)).ToListAsync();
            _dbContext.BookCopies.RemoveRange(copiesToDelete);
        }
        if (_bookIdsCreated.Any())
        {
            var booksToDelete = await _dbContext.Books.Where(b => _bookIdsCreated.Contains(b.Id)).ToListAsync();
            _dbContext.Books.RemoveRange(booksToDelete);
        }

        await _dbContext.SaveChangesAsync();

        _servicesScope.Dispose();
    }
}