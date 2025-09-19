using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Stayza.Application.Books;
using Stayza.Application.Books.Commands;
using Stayza.Application.Loans;
using Stayza.Domain.Books;
using Stayza.Domain.Loans;
using Stayza.Infrastructure.Persistence;
using Stayza.Infrastructure.Persistence.DataSeed;
using Stayza.Tests.Subcutaneous.Base;
using Stayza.Tests.Subcutaneous.Base.TestConstants;

namespace Stayza.Tests.Subcutaneous.BackgroundJobs;

public class CancelExpiredReservationsShould : IClassFixture<ApiFactory>, IAsyncLifetime
{
    private readonly ApiFactory _apiFactory;
    private IServiceScope _servicesScope;
    private IBooksRepository _repository;
    private BooksService _booksService;
    private TimeProvider _timeProvider;
    private LoansBackgroundJobs _sut;
    private ApplicationDbContext _dbContext;

    private readonly List<Guid> _bookIdsCreated = new();
    private readonly List<Guid> _bookCopyIdsCreated = new();
    private readonly List<Guid> _reservationIdsCreated = new();

    public CancelExpiredReservationsShould(ApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _servicesScope = _apiFactory.Services.CreateScope();
        _repository = _servicesScope.ServiceProvider.GetRequiredService<IBooksRepository>();
        _booksService = _servicesScope.ServiceProvider.GetService<BooksService>();
        _timeProvider = _servicesScope.ServiceProvider.GetRequiredService<TimeProvider>();
        _sut = _servicesScope.ServiceProvider.GetRequiredService<LoansBackgroundJobs>();
        _dbContext = _servicesScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }
    
    //HW: add an invariant test that would ensure that:
    // - cancelled reservations are also deleted
    // - reservations that are not expired are un-affected.
    [Fact]
    public async Task Cancel_expired_reservations()
    {
        // Arrange
        var book = new Book(
            title: Constants.Book.Title,
            author: Constants.Book.Author,
            isbn: Constants.Book.ISBN,
            id: Constants.Book.Id);
        var copy = book.AddCopy(book.Id);
        var reservation = copy.Reserve(
            userId: TestUserSeeder.TestUser1Id,
            utcNow: _timeProvider.GetUtcNow().AddDays(-60));

        await _repository.AddBook(book);

        _bookIdsCreated.Add(book.Id);
        _bookCopyIdsCreated.Add(copy.Id);
        _reservationIdsCreated.Add(reservation.Id);

        // Act
        await _sut.CancelExpiredReservations();

        // Assert
        var reservationFromDb = _dbContext.Reservations.Find(reservation.Id);
        reservationFromDb.Status.ShouldBe(ReservationStatus.Cancelled);
    }

    [Fact]
    public async Task Cancelled_reservations_are_deleted()
    {
        var book = await _booksService.AddBook(new AddBookCommand(
            Title: Constants.Book.Title,
            Author: Constants.Book.Author,
            ISBN: Constants.Book.ISBN));
        var copy = book.AddCopy(book.Id);
        var reservation = copy.Reserve(
            userId: TestUserSeeder.TestUser2Id,
            utcNow: _timeProvider.GetUtcNow().AddDays(10));

        copy.CancelReservation(reservation.Id, "book_copy is cancelled");

        _bookIdsCreated.Add(book.Id);
        _bookCopyIdsCreated.Add(copy.Id);
        _reservationIdsCreated.Add(reservation.Id);

        // Act
        await _sut.RemoveCancelledReservations();

        // Assert
        var cancelledReservation = await _dbContext.Reservations.FindAsync(reservation.Id);
        cancelledReservation.ShouldBeNull();
    }

    [Fact]
    public async Task Reservations_not_expired_are_unaffected()
    {
        // Arrange
        var book = new Book(
            title: Constants.Book.Title,
            author: Constants.Book.Author,
            isbn: Constants.Book.ISBN,
            id: Constants.Book.Id);
        var copy = book.AddCopy(book.Id);
        var reservation = copy.Reserve(
            userId: TestUserSeeder.TestUser2Id,
            utcNow: _timeProvider.GetUtcNow().AddDays(10));

        await _repository.AddBook(book);

        _bookIdsCreated.Add(book.Id);
        _bookCopyIdsCreated.Add(copy.Id);
        _reservationIdsCreated.Add(reservation.Id);

        // Act
        await _sut.CancelExpiredReservations();

        // Assert
        var reservationFromDb = await _dbContext.Reservations.FindAsync(reservation.Id);
        reservationFromDb.Status.ShouldBe(ReservationStatus.Pending);
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
        // Cleanup / Teardown
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