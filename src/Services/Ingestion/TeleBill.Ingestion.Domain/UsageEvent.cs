using TeleBill.BuildingBlocks.Domain;
namespace TeleBill.Ingestion.Domain;

public enum UsageType {Call = 1, Data = 2, Sms = 3}

public sealed class UsageEvent : Entity<Guid>
{
    private UsageEvent() { }
    public string ExternalEventId { get; private set; } = default!;
    public string Msisdn { get; private set; } = default!;
    public UsageType Type { get; private set; }
    public decimal Quantity { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public DateTimeOffset ReceivedAt { get; private set; }

    public static UsageEvent Create(string externalEventId, string msisdn, 
        UsageType type, decimal quantity, DateTimeOffset occurredAt, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(externalEventId))
            throw new DomainException("ExternalEventId is required.");
        if (quantity <= 0)
            throw new DomainException("Quantity must be positive.");
        if (type == UsageType.Sms && quantity != decimal.Truncate(quantity))
            throw new DomainException("SMS quantity must be a whole number.");

        
        return new UsageEvent
        {
            Id = Guid.CreateVersion7(),
            ExternalEventId = externalEventId, Msisdn = msisdn, Type = type, 
            Quantity = quantity, OccurredAt = occurredAt, ReceivedAt = now
        };
    }
}