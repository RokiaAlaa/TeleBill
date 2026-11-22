using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeleBill.Billing.Domain;

namespace TeleBill.Billing.Infrastructure.Persistence;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> b)
    {
        b.ToTable("subscriptions");
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);

        b.HasOne<Subscriber>().WithMany().HasForeignKey(x => x.SubscriberId);
        b.HasOne<Plan>().WithMany().HasForeignKey(x => x.PlanId);

        b.HasIndex(x => x.SubscriberId).IsUnique().HasFilter("\"Status\" = 'Active'");
    }
}
