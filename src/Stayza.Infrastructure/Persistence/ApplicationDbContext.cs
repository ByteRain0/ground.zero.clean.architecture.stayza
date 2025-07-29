using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Stayza.Domain.Books;
using Stayza.Domain.Loans;
using Stayza.Domain.Users;
using Stayza.Infrastructure.Persistence.DataSeed;

namespace Stayza.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<User>
{
    /// <summary>
    /// Left as public in order to allow having an external source run the migrations.
    /// </summary>
    /// <param name="options"></param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }

    public DbSet<BookCopy> BookCopies { get; set; }
    
    public DbSet<Reservation> Reservations { get; set; }

    public DbSet<Loan> Loans { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        builder.SeedBooks();
        builder.SeedBookCopies();

        base.OnModelCreating(builder);
        // in case you want to have a case_insensitive string comparison and not having .ToLower() everytime.
        //builder.HasCollation("case_insensitive", locale: "en-u-ks-primary", provider: "icu", deterministic: false);
    }
}