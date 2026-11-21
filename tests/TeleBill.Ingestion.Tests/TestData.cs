using TeleBill.Ingestion.Application.Usage;
using TeleBill.Ingestion.Domain;

namespace TeleBill.Ingestion.Tests;

public sealed class FakeTime(DateTimeOffset now) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => now;
}

public static class TestData
{
    public static readonly DateTimeOffset Now = new(2026, 10, 17, 12, 0, 0, TimeSpan.Zero);
    public static readonly TimeProvider Time = new FakeTime(Now);

    public static RecordUsageRequest Request(
        string id = "evt-1",
        string msisdn = "+201012345678",
        UsageType type = UsageType.Call,
        decimal quantity = 125,
        DateTimeOffset? occurredAt = null)
        => new(id, msisdn, type, quantity, occurredAt ?? Now.AddHours(-2));
}