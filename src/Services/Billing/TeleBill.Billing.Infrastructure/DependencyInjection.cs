using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TeleBill.Billing.Infrastructure.Messaging;

namespace TeleBill.Billing.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection s, IConfiguration c)
    {
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
                cfg.UseMessageRetry(r => r.Exponential(5,
                TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(2)));
                cfg.ConfigureEndpoints(ctx);
            });
        });
        return s;
    }
}