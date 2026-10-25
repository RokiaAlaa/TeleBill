using TeleBill.Ingestion.Domain;

namespace TeleBill.Ingestion.Application.Usage;

public interface IUsageEventPublisher
{
    Task PublishAsync(UsageEvent e, CancellationToken ct);
    Task PublishBatchAsync(IReadOnlyCollection<UsageEvent> events, CancellationToken ct);
}