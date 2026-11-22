using TeleBill.BuildingBlocks.Domain;

namespace TeleBill.Billing.Domain;

public enum SubscriptionStatus { Active = 1, Cancelled = 2 }

public sealed class Subscription : Entity<Guid>
{
    private Subscription() { }
    public Guid SubscriberId { get; private set; }
    public Guid PlanId { get; private set; }
    public DateTimeOffset StartedAt { get; private set; }
    public SubscriptionStatus Status { get; private set; }

    public static Subscription Create(Guid subscriberId, Guid planId, DateTimeOffset now) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            SubscriberId = subscriberId,
            PlanId = planId,
            StartedAt = now,
            Status = SubscriptionStatus.Active
        };

    public void Cancel()
    {
        if (Status == SubscriptionStatus.Cancelled)
            throw new DomainException("Subscription is already cancelled.");
        
        Status = SubscriptionStatus.Cancelled;
    }
}