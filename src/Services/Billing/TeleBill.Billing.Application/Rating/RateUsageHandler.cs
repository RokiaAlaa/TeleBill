using TeleBill.Billing.Domain;
using TeleBill.BuildingBlocks.Contracts;

namespace TeleBill.Billing.Application.Rating;

public sealed class RateUsageHandler(ISubscriptionReader subs, IBillingUnitOfWork uow,
    IUsageLedger ledger, IQuotaCache quota)
{
    public async Task HandleAsync(UsageRecorded m, CancellationToken ct)
    {
        var sub = await subs.GetActiveByMsisdnAsync(m.Msisdn, ct) 
                ?? throw new UnknownSubscriberException(m.Msisdn);
        var type = Enum.Parse<UsageType>(m.Type);
        var at = m.OccurredAt.UtcDateTime;
        var period = new DateOnly(at.Year, at.Month, 1);
        var rule = sub.Plan.RuleFor(type);

        decimal usedAfter;
        await using (var tx = await uow.BeginAsync(ct))
        {
            if (!await ledger.TryMarkProcessedAsync(m.UsageEventId, ct)) return;

            usedAfter = await ledger.AddToCounterAsync(sub.SubscriberId, period, type, m.Quantity, ct);
            var rated = RateEngine.Rate(rule, usedAfter - m.Quantity, m.Quantity);

            await ledger.AddRatedUsageAsync(new RatedUsageRecord(
                m.UsageEventId, sub.SubscriberId, period, type,
                m.Quantity, rated.IncludedQuantity, rated.OverageQuantity, rated.Charge,
                m.OccurredAt), ct);

            await tx.CommitAsync(ct);
        }
        await quota.RaiseToAsync(sub.SubscriberId, period, type, usedAfter);
    }
}