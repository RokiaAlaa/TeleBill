using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using TeleBill.Billing.Application.Rating;
using TeleBill.Billing.Domain;

namespace TeleBill.Billing.Infrastructure.Persistence;

internal sealed class UsageLedger(BillingDbContext db) : IUsageLedger
{
    public async Task<bool> TryMarkProcessedAsync(Guid id, CancellationToken ct)
    {
        await using var cmd = Cmd(
            "INSERT INTO processed_messages (message_id, processed_at) VALUES (@id, now()) ON CONFLICT DO NOTHING");
        cmd.Parameters.AddWithValue("id", id);
        return await cmd.ExecuteNonQueryAsync(ct) == 1;
    }

    public async Task<decimal> AddToCounterAsync(Guid s, DateOnly p, UsageType t, decimal q, CancellationToken ct)
    {
        await using var cmd = Cmd("""
        INSERT INTO usage_counters (subscriber_id, period, type, used) VALUES (@s, @p, @t, @q)
        ON CONFLICT (subscriber_id, period, type)
        DO UPDATE SET used = usage_counters.used + EXCLUDED.used
        RETURNING used
        """);

        cmd.Parameters.AddWithValue("s", s);
        cmd.Parameters.AddWithValue("p", p);
        cmd.Parameters.AddWithValue("t", t.ToString());
        cmd.Parameters.AddWithValue("q", q);
        return (decimal)(await cmd.ExecuteScalarAsync(ct))!;
    }

    public async Task AddRatedUsageAsync(RatedUsageRecord r, CancellationToken ct)
    {
        db.RatedUsages.Add(r);
        await db.SaveChangesAsync(ct);
    }

    private NpgsqlCommand Cmd(string sql) => new(sql,
        (NpgsqlConnection)db.Database.GetDbConnection(),
        (NpgsqlTransaction?)db.Database.CurrentTransaction?.GetDbTransaction());

}