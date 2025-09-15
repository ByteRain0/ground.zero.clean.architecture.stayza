using Stayza.Core.Specifications;
using Stayza.Domain.Books;

namespace Stayza.Application.Books.Specifications;

public class BookByIsbnSpecification : Specification<Book>
{
    public BookByIsbnSpecification(string isbn) 
        : base(book => book.ISBN.ToLower() == isbn.ToLower())
    {
        // In case you want to load
        AddInclude(book => book.Copies);
        
        // In case you want to order the entries
        AddOrderBy(book => book.ISBN);
    }
}