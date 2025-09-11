using System.Text.Json.Serialization;
using Ardalis.GuardClauses;
using Stayza.Core.Entity;
using Stayza.Core.Exceptions;
using Stayza.Domain.Loans;

namespace Stayza.Domain.Books;

public class Book : AggregateRoot
{
    [JsonInclude]
    public string Title { get; private set; }

    [JsonInclude]
    public string Author { get; private set; }

    [JsonInclude]
    public string ISBN { get; private set; }

    /// <summary>
    /// Book copies object referenced for easier db structure.
    /// </summary>
    private readonly HashSet<BookCopy> _copies = new();

    [Obsolete("Used only by ef core")]
    public Book()
    {
    }
    
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

    public IReadOnlyCollection<BookCopy> Copies => _copies.ToList().AsReadOnly();

    public BookCopy AddCopy(Guid bookCopyId)
    {
        Guard.Against.Null(_copies);
        
        if (_copies.Any(c => c.Id == bookCopyId))
            throw new InvalidOperationException("Copy already exists.");

        var bookCopy = new BookCopy(bookCopyId, Id);

        _copies.Add(bookCopy);

        return bookCopy;
    }

    public void RemoveCopy(Guid bookCopyId)
    {
        Guard.Against.Null(_copies);
        
        var copy = _copies.SingleOrDefault(c => c.Id == bookCopyId);

        if (copy == null)
            throw new EntityNotFoundException(
                entityType: nameof(BookCopy),
                searchKey: bookCopyId.ToString());

        if (copy.IsLoaned)
            throw new InvalidOperationException("Cannot remove a loaned copy.");

        _copies.Remove(copy);
    }
}