using TeleBill.Ingestion.Domain;

namespace TeleBill.Ingestion.Application.Usage;

public sealed record RecordUsageRequest(string ExternalEventId, string Msisdn,
    UsageType Type, decimal Quantity, DateTimeOffset OccurredAt);