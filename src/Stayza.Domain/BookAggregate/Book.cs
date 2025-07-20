using Stayza.Core.Entity;
using Stayza.Domain.BookCopyAggregate;

namespace Stayza.Domain.BookAggregate;

public class Book : AggregateRoot
{
    public string Title { get; private set; }

    public string Author { get; private set; }

    public string ISBN { get; private set; }

    private readonly List<BookCopy> _copies = new();
    
    public Book(
        string title,
        string author,
        string isbn,
        Guid id) : base(id)
    {
        Title = title;
        Author = author;
        ISBN = isbn;
    }

    public IReadOnlyCollection<BookCopy> Copies => _copies.AsReadOnly();

    public BookCopy AddCopy(Guid bookCopyId)
    {
        if (_copies.Any(c => c.Id == bookCopyId))
            throw new InvalidOperationException("Copy already exists.");

        var bookCopy = new BookCopy(bookCopyId, Id);
        
        _copies.Add(bookCopy);

        return bookCopy;
    }

    public void RemoveCopy(Guid bookCopyId)
    {
        var copy = _copies.SingleOrDefault(c => c.Id == bookCopyId);

        if (copy == null)
            throw new InvalidOperationException("Copy not found.");

        if (copy.IsLoaned)
            throw new InvalidOperationException("Cannot remove a loaned copy.");

        _copies.Remove(copy);
    }
}