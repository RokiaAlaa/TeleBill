using TeleBill.Billing.Domain;

namespace TeleBill.Billing.Application.Rating;

public interface IBillingTransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken ct);
}

public interface IBillingUnitOfWork
{
    Task<IBillingTransaction> BeginAsync(CancellationToken ct);
}

public interface IUsageLedger
{
    Task<bool> TryMarkProcessedAsync(Guid messageId, CancellationToken ct);
    Task<decimal> AddToCounterAsync(Guid subscriberId, DateOnly period, UsageType type, decimal qty, CancellationToken ct);
    Task AddRatedUsageAsync(RatedUsageRecord r, CancellationToken ct);
}

public sealed record SubscriptionInfo(Guid SubscriberId, Plan Plan);

public interface ISubscriptionReader
{
    Task<SubscriptionInfo?> GetActiveByMsisdnAsync(string msisdn, CancellationToken ct);
}

public interface IQuotaCache
{
    Task RaiseToAsync(Guid subscriberId, DateOnly period, UsageType type, decimal used);
}

public sealed class UnknownSubscriberException(string msisdn)
    : Exception($"Unknown subscriber {msisdn}");
