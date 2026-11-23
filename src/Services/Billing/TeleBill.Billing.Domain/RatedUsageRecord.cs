using TeleBill.BuildingBlocks.Domain;

namespace TeleBill.Billing.Domain;

public sealed class RatedUsageRecord : Entity<Guid>
{
    private RatedUsageRecord() { }

    public RatedUsageRecord(Guid usageEventId, Guid subscriberId, DateOnly period, UsageType type,
        decimal quantity, decimal includedQuantity, decimal overageQuantity, decimal charge,
        DateTimeOffset occurredAt)
    {
        Id = Guid.CreateVersion7();
        UsageEventId = usageEventId;
        SubscriberId = subscriberId;
        Period = period;
        Type = type;
        Quantity = quantity;
        IncludedQuantity = includedQuantity;
        OverageQuantity = overageQuantity;
        Charge = charge;
        OccurredAt = occurredAt;
    }

    public Guid UsageEventId{ get; private set; }
    public Guid SubscriberId{ get; private set; }
    public DateOnly Period{ get; private set; }
    public UsageType Type{ get; private set; }
    public decimal Quantity{ get; private set; }
    public decimal IncludedQuantity{ get; private set; }
    public decimal OverageQuantity{ get; private set; }
    public decimal Charge{ get; private set; }
    public DateTimeOffset OccurredAt{ get; private set; }
}