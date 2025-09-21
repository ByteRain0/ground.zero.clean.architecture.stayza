using Microsoft.Extensions.Caching.Hybrid;
using Stayza.Domain.Books;

namespace Stayza.Infrastructure.Cache;

public class BookRepositoryCache : IBooksRepository
{
    private readonly IBooksRepository _booksRepository;

    private readonly HybridCache _cache;
    
    public BookRepositoryCache(
        IBooksRepository booksRepository, 
        HybridCache cache)
    {
        _booksRepository = booksRepository;
        _cache = cache;
    }

    public Task<Book> AddBook(Book book)
    {
        return _booksRepository.AddBook(book);
    }

    public Task<Book> UpdateBook(Book book)
    {
        _cache.RemoveAsync($"book-{book.Id}");
        _cache.RemoveAsync($"book-{book.ISBN}");
        return _booksRepository.UpdateBook(book);
    }

    public async Task<Book> GetBookById(Guid id, CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(
            key: $"book-{id}",
            factory: async cancel => await _booksRepository.GetBookById(id, cancel),
            cancellationToken: cancellationToken);
    }

    public async Task<Book> GetBookByIsbn(string isbn, CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(
            key: $"book-{isbn}",
            factory: async cancel => await _booksRepository.GetBookByIsbn(isbn, cancel),
            cancellationToken: cancellationToken);
    }
}