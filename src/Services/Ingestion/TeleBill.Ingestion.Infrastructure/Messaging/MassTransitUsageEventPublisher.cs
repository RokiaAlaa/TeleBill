using MassTransit;
using TeleBill.BuildingBlocks.Contracts;
using TeleBill.Ingestion.Application.Usage;
using TeleBill.Ingestion.Domain;

namespace TeleBill.Ingestion.Infrastructure.Messaging;

internal sealed class MassTransitUsageEventPublisher(IPublishEndpoint bus) : IUsageEventPublisher
{
    public Task PublishAsync(UsageEvent e, CancellationToken ct) =>
        bus.Publish(new UsageRecorded(e.Id, e.Msisdn, e.Type.ToString(), e.Quantity, e.OccurredAt), ct);
}
