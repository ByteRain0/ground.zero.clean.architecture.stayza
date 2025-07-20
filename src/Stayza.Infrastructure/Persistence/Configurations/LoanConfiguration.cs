using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stayza.Domain.BookCopyAggregate;

namespace Stayza.Infrastructure.Persistence.Configurations;

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.BookCopyId)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.OwnsOne(x => x.TimeRange);
    }
}