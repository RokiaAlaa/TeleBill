using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TeleBill.Billing.Application.Rating;

namespace TeleBill.Billing.Infrastructure.Persistence;

internal sealed class BillingUnitOfWork(BillingDbContext db) : IBillingUnitOfWork
{
    public async Task<IBillingTransaction> BeginAsync(CancellationToken ct)
        => new EfBillingTransaction(await db.Database.BeginTransactionAsync(ct));
}

internal sealed class EfBillingTransaction(IDbContextTransaction tx) : IBillingTransaction
{
    public Task CommitAsync(CancellationToken ct) => tx.CommitAsync();
    public ValueTask DisposeAsync() => tx.DisposeAsync();
}
