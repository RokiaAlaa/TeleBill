using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using TeleBill.Billing.Application.Rating;
using TeleBill.Billing.Infrastructure.Caching;
using TeleBill.Billing.Infrastructure.Messaging;
using TeleBill.Billing.Infrastructure.Persistence;

namespace TeleBill.Billing.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection s, IConfiguration c)
    {
        s.AddDbContext<BillingDbContext>(o => o.UseNpgsql(c.GetConnectionString("Default")));
        s.AddScoped<ISubscriptionReader, SubscriptionReader>();
        s.AddScoped<IBillingUnitOfWork, BillingUnitOfWork>();
        s.AddScoped<IUsageLedger, UsageLedger>();

        s.AddSingleton<IConnectionMultiplexer>(_=>
        {
            var options = ConfigurationOptions.Parse(c["Redis:Connection"] ?? "localhost:6379");
            options.AbortOnConnectFail = false;
            return ConnectionMultiplexer.Connect(options);
        });
        s.AddSingleton<IQuotaCache, RedisQuotaCache>();

        s.AddMassTransit(x => {
            x.SetKebabCaseEndpointNameFormatter();
            x.AddConsumer<UsageRecordedConsumer>();
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(c["RabbitMq:Host"] ?? "localhost", "/", h =>
                {
                    h.Username(c["RabbitMq:User"] ?? "guest");
                    h.Password(c["RabbitMq:Password"] ?? "guest");
                });
                cfg.UseMessageRetry(r => 
                {
                    r.Exponential(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(2));
                    r.Ignore<UnknownSubscriberException>();
                });
                cfg.ConfigureEndpoints(ctx);
            });
        });
        return s;
    }
}