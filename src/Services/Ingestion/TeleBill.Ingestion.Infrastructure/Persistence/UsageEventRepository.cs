using TeleBill.Ingestion.Domain;
using TeleBill.Ingestion.Application.Usage;

namespace TeleBill.Ingestion.Infrastructure.Persistence;

public sealed class UsageEventRepository(IngestionDbContext db) : IUsageEventRepository
{
    public async Task AddAsync(UsageEvent e, CancellationToken ct)
        => await db.UsageEvents.AddAsync(e, ct);
}