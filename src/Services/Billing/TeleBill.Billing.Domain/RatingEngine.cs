namespace TeleBill.Billing.Domain;

public sealed record RatedUsage(decimal IncludedQuantity, decimal OverageQuantity, decimal Charge);

public static class RateEngine
{
    public static RatedUsage Rate(UsageRule rule, decimal usedBefore, decimal quantity)
    {
        var includedLeft = Math.Max(0, rule.IncludedQuantity - usedBefore);
        var included = Math.Min(quantity, includedLeft);
        var overage = quantity - included;
        if (overage == 0) return new(included, 0, 0);

        var overageBefore = Math.Max(0, usedBefore - rule.IncludedQuantity) / rule.BillingUnitSize;
        var overageAfter = (overageBefore + overage) / rule.BillingUnitSize;
        return new(included, overage, Math.Round(TieredCost(rule.Tiers, overageBefore, overageAfter), 4));
    }


    public static decimal TieredCost(IReadOnlyList<RateTier> tiers, decimal from, decimal to)
    {
        decimal total = 0, lower = 0;
        foreach (var t in tiers)
        {
            var upper = t.UpTo ?? decimal.MaxValue;
            var start = Math.Max(from, lower);
            var end = Math.Min(to, upper);
            if (end > start) total += (end - start) * t.RatePerUnit;
            if (to <= upper) break;
            lower = upper;
        }
        return total;
    }
}