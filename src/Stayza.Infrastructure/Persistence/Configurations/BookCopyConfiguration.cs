using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stayza.Domain.Loans;

namespace Stayza.Infrastructure.Persistence.Configurations;

public class BookCopyConfiguration : IEntityTypeConfiguration<BookCopy>
{
    public void Configure(EntityTypeBuilder<BookCopy> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();
        
        builder.Property(x => x.BookId)
            .IsRequired();
        
        builder
            .HasMany(x => x.Reservations)
            .WithOne()
            .HasForeignKey(x => x.BookCopyId);
        
        builder
            .HasOne(x => x.CurrentLoan)
            .WithOne()
            .HasForeignKey<Loan>(l => l.BookCopyId)
            .IsRequired(false);
        
        builder
            .Ignore(x => x.DomainEvents)
            .Ignore(x => x.IsAvailable)
            .Ignore(x => x.IsLoaned)
            .Ignore(x => x.ActiveReservation)
            .Ignore(x => x.PendingReservations);

    }
}