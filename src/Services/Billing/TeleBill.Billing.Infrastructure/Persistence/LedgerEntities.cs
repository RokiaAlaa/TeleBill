using TeleBill.Billing.Domain;

namespace TeleBill.Billing.Infrastructure.Persistence;

public sealed class UsageCounter
{
    public Guid SubscriberId { get; set; }
    public DateOnly Period { get; set; }
    public UsageType Type { get; set; }
    public decimal Used { get; set; }
}

public sealed class ProcessedMessage
{
    public Guid MessageId { get; set; }
    public DateTimeOffset ProcessedAt { get; set; }
}