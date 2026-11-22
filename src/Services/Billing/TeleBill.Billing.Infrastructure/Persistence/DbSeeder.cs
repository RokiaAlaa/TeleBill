using Microsoft.EntityFrameworkCore;
using TeleBill.Billing.Domain;

namespace TeleBill.Billing.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(BillingDbContext db, DateTimeOffset now, CancellationToken ct = default)
    {
        if(await db.Plans.AnyAsync(ct)) return;

        var basic = Plan.Create("Basic", 150m,
        [
            new UsageRule(UsageType.Call, 18000, 60, [new RateTier(null, 0.50m)]),
            new UsageRule(UsageType.Data, 5_242_880, 1024, 
                [new RateTier(1024, 0.20m), new RateTier(null, 0.10m)]),
            new UsageRule(UsageType.Sms, 100, 1, [new RateTier(null, 0.25m)]),

        ]);

        var subscriber = Subscriber.Create("+201012345678", "Test Subscriber", now);
        var subscription = Subscription.Create(subscriber.Id, basic.Id, now);

        db.Plans.Add(basic);
        db.Subscribers.Add(subscriber);
        db.Subscriptions.Add(subscription);
        await db.SaveChangesAsync(ct);
    }
}