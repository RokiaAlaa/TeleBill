using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeleBill.Billing.Domain;

namespace TeleBill.Billing.Infrastructure.Persistence;

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> b)
    {
        b.ToTable("plans");
        b.Property(x => x.Name).HasMaxLength(50).IsRequired();
        b.Property(x => x.MonthlyFee).HasPrecision(18, 2);
        b.Property(x => x.Currency).HasMaxLength(3).IsRequired();
        b.OwnsMany(x => x.Rules, r => 
        {
            r.ToJson();
            r.OwnsMany(x => x.Tiers);
        });
    }
}
