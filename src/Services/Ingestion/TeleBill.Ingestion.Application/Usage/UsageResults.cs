namespace TeleBill.Ingestion.Application.Usage;

public sealed record RecordUsageResult(Guid? Id, bool IsDuplicate);

public sealed record RecordUsageBatchRequest(IReadOnlyList<RecordUsageRequest> Items);

public sealed record RejectedItem(int Index, string[] Errors);

public sealed record RecordUsageBatchResult(int Accepted, int Duplicates, IReadOnlyCollection<RejectedItem> Rejected);
