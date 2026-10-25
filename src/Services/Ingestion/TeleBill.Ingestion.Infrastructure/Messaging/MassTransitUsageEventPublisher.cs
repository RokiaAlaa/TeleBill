using MassTransit;
using TeleBill.BuildingBlocks.Contracts;
using TeleBill.Ingestion.Application.Usage;
using TeleBill.Ingestion.Domain;

namespace TeleBill.Ingestion.Infrastructure.Messaging;

internal sealed class MassTransitUsageEventPublisher(IPublishEndpoint bus) : IUsageEventPublisher
{
    public Task PublishAsync(UsageEvent e, CancellationToken ct) =>
        bus.Publish(ToMessage(e), ct);

    public Task PublishBatchAsync(IReadOnlyCollection<UsageEvent> events, CancellationToken ct) =>
        bus.PublishBatch(events.Select(ToMessage), ct);

    private static UsageRecorded ToMessage(UsageEvent e) =>
        new (e.Id, e.Msisdn, e.Type.ToString(), e.Quantity, e.OccurredAt);
}
