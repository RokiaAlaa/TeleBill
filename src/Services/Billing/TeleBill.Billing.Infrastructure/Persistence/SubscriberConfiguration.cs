using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeleBill.Billing.Domain;

namespace TeleBill.Billing.Infrastructure.Persistence;

public class SubscriberConfiguration : IEntityTypeConfiguration<Subscriber>
{
    public void Configure(EntityTypeBuilder<Subscriber> b)
    {
        b.ToTable("subscribers");
        b.Property(x => x.Msisdn).HasMaxLength(16).IsRequired();
        b.Property(x => x.FullName).HasMaxLength(100).IsRequired();
        b.HasIndex(x => x.Msisdn).IsUnique();
    }
}
