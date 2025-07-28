using Stayza.Domain.Books;
using Stayza.Domain.Loans;
using Stayza.Tests.Unit.Utils.TestConstants;

namespace Stayza.Tests.Unit.Utils;

internal static class TestBooksFactory
{
    public static Book CreateBook(
        string? title = null,
        string? author = null,
        string? isbn = null,
        Guid? id = null)
    {
        return new Book(
            title: title ?? Constants.Book.Title,
            author: author ?? Constants.Book.Author,
            isbn: isbn ?? Constants.Book.ISBN,
            id: id ?? Constants.Book.Id);
    }

    public static BookCopy CreateBookCopy(
        Guid? bookId = null,
        Guid? id = null)
    {
        return new BookCopy(
            id: id ?? Constants.BookCopy.BookCopyId,
            bookId: bookId ?? Constants.Book.Id);
    }
}