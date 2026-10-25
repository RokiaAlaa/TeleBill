using FluentValidation;
using TeleBill.BuildingBlocks.Domain;
using TeleBill.Ingestion.Domain;

namespace TeleBill.Ingestion.Application.Usage;

public sealed class RecordUsageBatchHandler(IValidator<RecordUsageRequest> validator,
    IUsageEventRepository repo, IUnitOfWork uow, IUsageEventPublisher publisher, TimeProvider time)
{
    public const int MaxBatchSize = 1000;

    public async Task<RecordUsageBatchResult> HandleAsync(RecordUsageBatchRequest batch, CancellationToken ct)
    {
        var items = batch.Items;
        if(items.Count == 0)
            throw new ValidationException("Batch cannot be empty.");
        if(items.Count > MaxBatchSize)
            throw new ValidationException($"Batch cannot exceed {MaxBatchSize} items.");

        var rejected = new List<RejectedItem>();

        var valid = new List<(int Index, RecordUsageRequest Item)>();
        for (var idx = 0; idx < items.Count; idx++)
        {
            var item = items[idx];
            if(item is null)
            {
                rejected.Add(new RejectedItem(idx, ["Item is null."]));
                continue;
            }

            var result = await validator.ValidateAsync(item, ct);
            if(result.IsValid)
                valid.Add((idx, item));
            else
                rejected.Add(new RejectedItem(idx, result.Errors.Select(e => e.ErrorMessage).ToArray()));
        }

        var distinct = valid.DistinctBy(x => x.Item.ExternalEventId).ToList();
        var duplicates = valid.Count - distinct.Count;

        var existing = await repo.GetExistingExternalIdsAsync(
            distinct.Select(x => x.Item.ExternalEventId).ToArray(), ct);

        var now = time.GetUtcNow();
        var fresh = new List<UsageEvent>();

        foreach (var (index, r) in distinct)
        {
            if(existing.Contains(r.ExternalEventId))
            {
                duplicates++;
                continue;
            }
            try
            {
                fresh.Add(UsageEvent.Create(r.ExternalEventId, r.Msisdn, r.Type, r.Quantity, r.OccurredAt, now));
            }
            catch (DomainException ex)
            {
                rejected.Add(new RejectedItem(index, [ex.Message]));
            }
        }

        if (fresh.Count > 0)
        {
            await repo.AddRangeAsync(fresh, ct);
            await uow.SaveChangesAsync(ct);
            await publisher.PublishBatchAsync(fresh, ct);
        }

        return new RecordUsageBatchResult(fresh.Count, duplicates,
            rejected.OrderBy(x => x.Index).ToList());
    }
}