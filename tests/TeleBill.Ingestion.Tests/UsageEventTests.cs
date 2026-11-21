using Shouldly;
using TeleBill.BuildingBlocks.Domain;
using TeleBill.Ingestion.Domain;

namespace TeleBill.Ingestion.Tests;

public class UsageEventTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-125.5)]
    public void Create_throws_when_quantity_is_not_positive(double quantity)
    {
        Should.Throw<DomainException>(() => UsageEvent.Create(
            "evt-1", "+201012345678", UsageType.Call, (decimal) quantity, TestData.Now, TestData.Now));
    }

    [Fact]
    public void Create_throws_when_sms_quantity_is_fractional()
    {
        Should.Throw<DomainException>(() => UsageEvent.Create(
            "evt-1", "+201012345678", UsageType.Sms, 1.5m, TestData.Now, TestData.Now));
    }

    [Fact]
    public void Create_throws_when_external_id_is_blank()
    {
        Should.Throw<DomainException>(() => UsageEvent.Create(
            " ", "+201012345678", UsageType.Call, 10, TestData.Now, TestData.Now));
    }

    [Fact]
    public void Create_returns_event_with_generated_id_for_valid_input()
    {
        var e = UsageEvent.Create(
            "evt-1", "+201012345678", UsageType.Data, 2048, TestData.Now, TestData.Now);
        
        e.Id.ShouldNotBe(Guid.Empty);
        e.ExternalEventId.ShouldBe("evt-1");
        e.Quantity.ShouldBe(2048m);
    }
}