using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeleBill.Billing.Domain;

namespace TeleBill.Billing.Infrastructure.Persistence;

public class UsageCounterConfiguration : IEntityTypeConfiguration<UsageCounter>
{
    public void Configure(EntityTypeBuilder<UsageCounter> b)
    {
        b.ToTable("usage_counters");
        b.Property(x => x.SubscriberId).HasColumnName("subscriber_id");
        b.Property(x => x.Period).HasColumnName("period");
        b.Property(x => x.Type).HasColumnName("type").HasConversion<string>().HasMaxLength(10);
        b.Property(x => x.Used).HasColumnName("used").HasPrecision(18, 3);
        b.HasKey(x => new { x.SubscriberId, x.Period, x.Type });
    }
}

public class ProcessedMessageConfiguration : IEntityTypeConfiguration<ProcessedMessage>
{
    public void Configure(EntityTypeBuilder<ProcessedMessage> b)
    {
        b.ToTable("processed_messages");
        b.HasKey(x => x.MessageId);
        b.Property(x => x.MessageId).HasColumnName("message_id").ValueGeneratedNever();
        b.Property(x => x.ProcessedAt).HasColumnName("processed_at");
    }
}

public class RatedUsageRecordConfiguration : IEntityTypeConfiguration<RatedUsageRecord>
{
    public void Configure(EntityTypeBuilder<RatedUsageRecord> b)
    {
        b.ToTable("rated_usage");
        b.Property(x => x.Type).HasColumnName("type").HasConversion<string>().HasMaxLength(10);
        b.Property(x => x.Quantity).HasPrecision(18, 3);
        b.Property(x => x.IncludedQuantity).HasPrecision(18, 3);
        b.Property(x => x.OverageQuantity).HasPrecision(18, 3);
        b.Property(x => x.Charge).HasPrecision(18, 4);
        b.HasIndex(x => x.UsageEventId).IsUnique();
        b.HasIndex(x => new { x.SubscriberId, x.Period });
    }
}