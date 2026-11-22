using Microsoft.EntityFrameworkCore;
using TeleBill.Billing.Domain;

namespace TeleBill.Billing.Infrastructure.Persistence;

public sealed class BillingDbContext(DbContextOptions<BillingDbContext> options) : DbContext(options)
{
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<Subscriber> Subscribers => Set<Subscriber>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    protected override void OnModelCreating(ModelBuilder mb) => mb.ApplyConfigurationsFromAssembly(typeof(BillingDbContext).Assembly);
}