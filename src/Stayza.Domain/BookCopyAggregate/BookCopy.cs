using Stayza.Core.Entity;
using Stayza.Domain.BookAggregate;
using Stayza.Domain.BookCopyAggregate.Events;
using Stayza.Domain.BookCopyAggregate.Exceptions;

namespace Stayza.Domain.BookCopyAggregate;

public class BookCopy : AggregateRoot
{
    public Guid Id { get; private set; }

    public Guid BookId { get; set; }
    
    public Book Book { get; private set; }

    private readonly List<Reservation> _reservations = new();
    
    private Loan? _currentLoan;

    public bool IsAvailable => _currentLoan == null || _currentLoan.IsReturned;
    
    public bool IsLoaned => _currentLoan != null && !_currentLoan.IsReturned;

    public BookCopy(Guid id, Guid bookId)
    {
        Id = id;
        BookId = bookId;
    }
    
    public void Reserve(
        Guid userId,
        DateTimeOffset utcNow)
    {
        if (!IsAvailable)
            throw new BookNotAvailableForReservation();
        
        if (_reservations.Any(r => r.UserId == userId && r.Status == ReservationStatus.Active))
            throw new ReservationAlreadyExistsException(userId);

        _reservations.Add(new Reservation(
            userId: userId,
            reservedAt: utcNow,
            bookCopyId: Id,
            id: Guid.NewGuid()));
    }
    
    public void StartLoan(
        Guid userId,
        DateTimeOffset utcNow)
    {
        if (IsLoaned)
            throw new BookCopyAlreadyLoanedException(userId);

        var hasReservation = _reservations.Any(r => r.UserId == userId && r.Status == ReservationStatus.Active);

        if (!hasReservation)
            throw new ReservationNotFound();

        _currentLoan = new Loan(
            copyId: Id,
            userId: userId,
            loanDate: utcNow,
            dueDate: utcNow.AddDays(14),
            id: Guid.NewGuid());

        AddDomainEvent(new BookLoanedEvent(
            BookCopyId: Id,
            UserId: userId,
            LoanId: _currentLoan.Id,
            LoanDate: _currentLoan.LoanDate,
            DueDate: _currentLoan.DueDate));
    }
    
    public void Return(DateTimeOffset utcNow)
    {
        if (!IsLoaned)
            throw new InvalidOperationException("Copy is not loaned.");
        
        _currentLoan!.MarkAsReturned(returnedAt: utcNow);
        
        AddDomainEvent(new BookReturnedEvent(
            BookCopyId: Id, 
            LoanId: _currentLoan.Id,
            ReturnDate: utcNow));
    }
}