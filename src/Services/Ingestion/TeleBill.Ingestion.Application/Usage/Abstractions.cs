using TeleBill.Ingestion.Domain;

namespace TeleBill.Ingestion.Application.Usage;

public interface IUsageEventRepository
{
    Task AddAsync(UsageEvent e, CancellationToken ct);
    Task AddRangeAsync(IEnumerable<UsageEvent> events, CancellationToken ct);
    Task<bool> ExistsAsync(string externalEventId, CancellationToken ct);
    Task<HashSet<string>> GetExistingExternalIdsAsync(string[] externalEventIds, CancellationToken ct);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}