using Microsoft.EntityFrameworkCore;
using Stayza.Domain.BookAggregate;
using Stayza.Domain.BookCopyAggregate;

namespace Stayza.Infrastructure.Persistence.DataSeed;

public static class BooksAndCopiesSeed
{
    public static void SeedBooks(this ModelBuilder builder)
    {
        builder.Entity<Book>().HasData(new List<Book>
        {
            new(title: "Righting Software",
                author: "Juval Löwy",
                isbn: "9781642955706",
                id: Guid.Parse("e0c22377-4fe9-4d95-89bb-61f498c8c1be")),

            new(title: "Enterprise Integration Patterns",
                author: "Gregor Hohpe, Bobby Woolf",
                isbn: "9780321200686",
                id: Guid.Parse("6106dfed-7f89-43f0-8a5d-22775be15211")),

            new(title: "The Architect's Elevator",
                author: "Gregor Hohpe",
                isbn: "9781642955706",
                id: Guid.Parse("c5e0a48f-9bcc-4e39-8474-81df953756a2")),

            new(title: "Balancing Coupling in Software Design",
                author: "Vlad Khononov",
                isbn: "9781617298413",
                id: Guid.Parse("cb295459-2d9f-40ef-b423-62375b5db562")),

            new(title: "The Pragmatic Programmer",
                author: "Andrew Hunt, David Thomas",
                isbn: "9780135957059",
                id: Guid.Parse("61415c09-6dd2-42d8-a1e4-03774558a4b5"))
        });
    }

    public static void SeedBookCopies(this ModelBuilder builder)
    {
        builder.Entity<BookCopy>().HasData(new List<BookCopy>
        {
            new(id: Guid.Parse("95ea8f22-ad65-4278-ba50-eb96af41397e"),
                bookId: Guid.Parse("e0c22377-4fe9-4d95-89bb-61f498c8c1be")),

            new(id: Guid.Parse("16d7bbf1-474d-4036-a81b-e3ce0b64a361"),
                bookId: Guid.Parse("6106dfed-7f89-43f0-8a5d-22775be15211")),

            new(id: Guid.Parse("f0d7cdc7-8fe1-4a5f-9ac3-94ae0587d9a5"),
                bookId: Guid.Parse("c5e0a48f-9bcc-4e39-8474-81df953756a2")),

            new(id: Guid.Parse("535e94c0-e2e9-4375-8f48-3caf42e5f58c"),
                bookId: Guid.Parse("cb295459-2d9f-40ef-b423-62375b5db562")),

            new(id: Guid.Parse("2c80e65f-f8a5-4c2f-84a0-773fd71293b5"),
                bookId: Guid.Parse("61415c09-6dd2-42d8-a1e4-03774558a4b5"))
        });
    }
}