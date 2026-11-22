using TeleBill.BuildingBlocks.Domain;

namespace TeleBill.Billing.Domain;

public enum UsageType {Call = 1, Data = 2, Sms = 3}

public sealed class RateTier
{
    private RateTier() { }
    public RateTier(decimal? upTo, decimal ratePerUnit) { UpTo = upTo; RatePerUnit = ratePerUnit; }

    public decimal? UpTo { get; private set; }
    public decimal RatePerUnit { get; private set; }
}

public sealed class UsageRule
{
    private UsageRule() { }
    public UsageRule(UsageType type, decimal included, decimal billingUnitSize, IEnumerable<RateTier> tiers)
    {
        Type = type;
        IncludedQuantity = included;
        BillingUnitSize = billingUnitSize;
        Tiers = tiers.ToList();
    }

    public UsageType Type { get; private set; }
    public decimal IncludedQuantity { get; private set; }
    public decimal BillingUnitSize { get; private set; }
    public List<RateTier> Tiers { get; private set; } = new();
}

public sealed class Plan : Entity<Guid>
{
    private Plan() { }
    public string Name { get; private set; } = default!;
    public decimal MonthlyFee { get; private set; }
    public string Currency { get; private set; } = "EGP";
    public List<UsageRule> Rules { get; private set; } = new();

    public static Plan Create(string name, decimal fee, IEnumerable<UsageRule> rules)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Plan name is required.");
        if (fee < 0)
            throw new DomainException("Monthly fee cannot be negative.");

        return new Plan { Id = Guid.CreateVersion7(), Name = name, MonthlyFee = fee, Rules = rules.ToList() };
    }

    public UsageRule RuleFor(UsageType t) => Rules.Single(r => r.Type == t);
}