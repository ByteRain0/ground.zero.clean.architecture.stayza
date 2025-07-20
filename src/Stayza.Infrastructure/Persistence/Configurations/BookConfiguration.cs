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
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Author)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ISBN)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasMany<BookCopy>()
            .WithOne()
            .HasForeignKey(x => x.BookId);

        builder.Ignore(x => x.DomainEvents);
    }
}