using FluentValidation;
using TeleBill.Ingestion.Domain;

namespace TeleBill.Ingestion.Application.Usage;

public sealed class RecordUsageHandler(IValidator<RecordUsageRequest> validator, IUsageEventRepository repo, IUnitOfWork uow, TimeProvider time)
{
    public async Task<Guid> HandleAsync(RecordUsageRequest r, CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(r, ct);
        var e = UsageEvent.Create(r.ExternalEventId, r.Msisdn, r.Type, r.Quantity, r.OccurredAt, time.GetUtcNow());
        await repo.AddAsync(e, ct);
        await uow.SaveChangesAsync(ct);
        return e.Id;
    }
}