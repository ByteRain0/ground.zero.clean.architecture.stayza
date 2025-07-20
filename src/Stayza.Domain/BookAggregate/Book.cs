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

    public void AddCopy(Guid copyId)
    {
        if (_copies.Any(c => c.Id == copyId))
            throw new InvalidOperationException("Copy already exists.");

        _copies.Add(new BookCopy(copyId, Id));
    }

    public void RemoveCopy(Guid copyId)
    {
        var copy = _copies.SingleOrDefault(c => c.Id == copyId);

        if (copy == null)
            throw new InvalidOperationException("Copy not found.");

        if (copy.IsLoaned)
            throw new InvalidOperationException("Cannot remove a loaned copy.");

        _copies.Remove(copy);
    }
}