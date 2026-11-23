using System.Globalization;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using TeleBill.Billing.Application.Rating;
using TeleBill.Billing.Domain;

namespace TeleBill.Billing.Infrastructure.Caching;

internal sealed class RedisQuotaCache(IConnectionMultiplexer redis, ILogger<RedisQuotaCache> log) : IQuotaCache
{
    private const string RaiseScript = """
        local cur = tonumber(redis.call('GET', KEYS[1]))
        if cur == nil or cur < tonumber(ARGV[1]) then
            redis.call('SET', KEYS[1], ARGV[1], 'EX', ARGV[2])
            return 1
        end
        return 0
        """;

    private static readonly int TtlSeconds = (int)TimeSpan.FromDays(62).TotalSeconds;

    public async Task RaiseToAsync(Guid subscriberId, DateOnly period, UsageType type, decimal used)
    {
        try
        {
            var key = $"quota:{subscriberId}:{period.ToString("yyyy-MM", CultureInfo.InvariantCulture)}:{type}";
            await redis.GetDatabase().ScriptEvaluateAsync(
                RaiseScript,
                [(RedisKey)key],
                [(RedisValue)used.ToString(CultureInfo.InvariantCulture), (RedisValue)TtlSeconds]);
        }
        catch (Exception ex)
        {
            log.LogWarning(ex, "Failed to update quota cache for {SubscriberId}", subscriberId);
        }
    }
}