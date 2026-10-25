using Microsoft.EntityFrameworkCore;
using TeleBill.Ingestion.Domain;
using TeleBill.Ingestion.Application.Usage;

namespace TeleBill.Ingestion.Infrastructure.Persistence;

public sealed class UsageEventRepository(IngestionDbContext db) : IUsageEventRepository
{
    public async Task AddAsync(UsageEvent e, CancellationToken ct)
        => await db.UsageEvents.AddAsync(e, ct);

    public async Task AddRangeAsync(IEnumerable<UsageEvent> events, CancellationToken ct)
        => await db.UsageEvents.AddRangeAsync(events, ct);

    public Task<bool> ExistsAsync(string externalEventId, CancellationToken ct)
        => db.UsageEvents.AnyAsync(x => x.ExternalEventId == externalEventId, ct);

    public async Task<HashSet<string>> GetExistingExternalIdsAsync(string[] externalEventIds, CancellationToken ct)
    {
        var found = await db.UsageEvents
            .AsNoTracking()
            .Where(x => externalEventIds.Contains(x.ExternalEventId))
            .Select(x => x.ExternalEventId)
            .ToListAsync(ct);
        
        return found.ToHashSet();
    }
}