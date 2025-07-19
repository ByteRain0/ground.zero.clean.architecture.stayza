using Microsoft.EntityFrameworkCore;
using Stayza.Domain.BookAggregate;
using Stayza.Domain.BookCopyAggregate;
using Stayza.Domain.UserAggregate;

namespace Stayza.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public DbSet<Book> Books { get; set; }

    public DbSet<BookCopy> BookCopies { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Reservation> Reservations { get; set; }

    public DbSet<Loan> Loans { get; set; }
}