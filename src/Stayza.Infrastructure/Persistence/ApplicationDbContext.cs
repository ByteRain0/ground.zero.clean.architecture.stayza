using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Stayza.Domain.Books;
using Stayza.Domain.Loans;
using Stayza.Domain.Users;
using Stayza.Infrastructure.Persistence.DataSeed;
using Stayza.Infrastructure.Persistence.Interceptors;
using TickerQ.EntityFrameworkCore.Configurations;

namespace Stayza.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<User>
{
    private readonly PublishDomainEventsInterceptor _publishDomainEventsInterceptor;
    
    /// <summary>
    /// Left as public in order to allow having an external source run the migrations.
    /// </summary>
    /// <param name="options"></param>
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        PublishDomainEventsInterceptor publishDomainEventsInterceptor)
        : base(options)
    {
        _publishDomainEventsInterceptor = publishDomainEventsInterceptor;
    }

    public DbSet<Book> Books { get; set; }

    public DbSet<BookCopy> BookCopies { get; set; }
    
    public DbSet<Reservation> Reservations { get; set; }

    public DbSet<Loan> Loans { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.HasDefaultSchema("library");
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(TimeTickerConfigurations).Assembly);
        builder.SeedBooks();
        builder.SeedBookCopies();
        
        // in case you want to have a case_insensitive string comparison and not having .ToLower() everytime.
        //builder.HasCollation("case_insensitive", locale: "en-u-ks-primary", provider: "icu", deterministic: false);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_publishDomainEventsInterceptor);
        base.OnConfiguring(optionsBuilder);
    }
}