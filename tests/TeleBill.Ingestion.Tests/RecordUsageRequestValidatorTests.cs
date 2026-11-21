using Shouldly;
using TeleBill.Ingestion.Application.Usage;

namespace TeleBill.Ingestion.Tests;

public class RecordUsageRequestValidatorTests
{
    private readonly RecordUsageRequestValidator _sut = new(TestData.Time);

    [Fact]
    public void Valid_request_passes()
    {
        _sut.Validate(TestData.Request()).IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("0101234")]
    [InlineData("201012345678")]
    [InlineData("+0123456789")]
    [InlineData("+20abc")]
    public void Invalid_msisdn_fails(string msisdn)
    {
        var result = _sut.Validate(TestData.Request(msisdn: msisdn));

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Msisdn");
    }

    [Fact]
    public void Future_occurredAt_fails()
    {
        var result = _sut.Validate(TestData.Request(occurredAt: TestData.Now.AddHours(1)));

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "OccurredAt");
    }

    [Fact]
    public void OccurredAt_within_five_minutes_tolerance_passes()
    {
        var result = _sut.Validate(TestData.Request(occurredAt: TestData.Now.AddMinutes(4)));

        result.IsValid.ShouldBeTrue();
    }
}