using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeleBill.Ingestion.Domain;

namespace TeleBill.Ingestion.Infrastructure.Persistence;

public class UsageEventConfiguration : IEntityTypeConfiguration<UsageEvent>
{
    public void Configure(EntityTypeBuilder<UsageEvent> b)
    {
        b.ToTable("usage_events");
        b.Property(x => x.ExternalEventId).HasMaxLength(100).IsRequired();
        b.Property(x => x.Msisdn).HasMaxLength(16).IsRequired();
        b.Property(x => x.Type).HasConversion<string>().HasMaxLength(10);
        b.Property(x => x.Quantity).HasPrecision(18,3);
        b.HasIndex(x => x.ExternalEventId).IsUnique();
        b.HasIndex(x => new { x.Msisdn, x.OccurredAt });
    }
}