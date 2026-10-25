using FluentValidation;
using TeleBill.Ingestion.Domain;

namespace TeleBill.Ingestion.Application.Usage;

public sealed class RecordUsageHandler(IValidator<RecordUsageRequest> validator,
    IUsageEventRepository repo, IUnitOfWork uow, IUsageEventPublisher publisher, TimeProvider time)
{
    public async Task<RecordUsageResult> HandleAsync(RecordUsageRequest r, CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(r, ct);

        if (await repo.ExistsAsync(r.ExternalEventId, ct))
            return new RecordUsageResult(null, true);

        var e = UsageEvent.Create(r.ExternalEventId, r.Msisdn, r.Type, r.Quantity, r.OccurredAt, time.GetUtcNow());
        await repo.AddAsync(e, ct);

        try
        {
            await uow.SaveChangesAsync(ct);
        }
        catch (DuplicateUsageEventException)
        {
            return new RecordUsageResult(null, true);
        }

        await publisher.PublishAsync(e, ct);
        return new RecordUsageResult(e.Id, false);
    }
}