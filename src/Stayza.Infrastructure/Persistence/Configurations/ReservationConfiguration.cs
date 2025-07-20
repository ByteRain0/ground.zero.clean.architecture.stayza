using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stayza.Domain.BookCopyAggregate;

namespace Stayza.Infrastructure.Persistence.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.BookCopyId)
            .IsRequired();

        builder.Property(x => x.ReservedAt)
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .IsRequired();
    }
}