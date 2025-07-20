using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stayza.Domain.BookCopyAggregate;

namespace Stayza.Infrastructure.Persistence.Configurations;

public class BookCopyConfiguration : IEntityTypeConfiguration<BookCopy>
{
    public void Configure(EntityTypeBuilder<BookCopy> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.BookId)
            .IsRequired();

        builder.HasMany<Reservation>()
            .WithOne()
            .HasForeignKey(x => x.BookCopyId);

        builder.HasMany<Loan>()
            .WithOne()
            .HasForeignKey(x => x.BookCopyId);

        builder.Ignore(x => x.DomainEvents);
    }
}