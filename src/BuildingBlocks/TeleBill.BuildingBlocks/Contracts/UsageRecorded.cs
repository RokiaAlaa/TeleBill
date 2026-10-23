namespace TeleBill.BuildingBlocks.Contracts;

public sealed record UsageRecorded(Guid UsageEventId, string Msisdn, string Type, 
    decimal Quantity, DateTimeOffset OccurredAt);
