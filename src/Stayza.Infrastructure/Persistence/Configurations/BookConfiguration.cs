using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stayza.Domain.BookAggregate;
using Stayza.Domain.BookCopyAggregate;

namespace Stayza.Infrastructure.Persistence.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            // If you want to make sure title comparison is case in-sensitive use this approach:
            // https://www.npgsql.org/efcore/misc/collations-and-case-sensitivity.html?tabs=fluent-api
            //.UseCollation("case_insensitive")
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Author)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ISBN)
            .IsRequired()
            .HasMaxLength(14);

        builder.Ignore(x => x.DomainEvents);
    }
}