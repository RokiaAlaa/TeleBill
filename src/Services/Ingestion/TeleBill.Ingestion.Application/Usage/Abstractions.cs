using TeleBill.Ingestion.Domain;

namespace TeleBill.Ingestion.Application.Usage;

public interface IUsageEventRepository
{
    Task AddAsync(UsageEvent e, CancellationToken ct);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}