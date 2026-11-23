using Microsoft.EntityFrameworkCore;
using TeleBill.Billing.Application.Rating;
using TeleBill.Billing.Domain;

namespace TeleBill.Billing.Infrastructure.Persistence;

internal sealed class SubscriptionReader(BillingDbContext db) : ISubscriptionReader
{
    public async Task<SubscriptionInfo?> GetActiveByMsisdnAsync(string msisdn, CancellationToken ct)
    {
        var row = await (
            from sub in db.Subscriptions.AsNoTracking()
            join subscriber in db.Subscribers.AsNoTracking() on sub.SubscriberId equals subscriber.Id
            join plan in db.Plans.AsNoTracking() on sub.PlanId equals plan.Id
            where subscriber.Msisdn == msisdn && sub.Status == SubscriptionStatus.Active
            select new { SubscriberId = subscriber.Id, Plan = plan}
        ).FirstOrDefaultAsync(ct);

        return row is null ? null : new SubscriptionInfo(row.SubscriberId, row.Plan);
    }
}