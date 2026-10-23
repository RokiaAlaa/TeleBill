using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TeleBill.Ingestion.Infrastructure.Messaging;
using TeleBill.Ingestion.Infrastructure.Persistence;
using TeleBill.Ingestion.Application.Usage;

namespace TeleBill.Ingestion.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection s, IConfiguration c)
    {
        s.AddDbContext<IngestionDbContext>(o => o.UseNpgsql(c.GetConnectionString("Default")));
        s.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<IngestionDbContext>());
        s.AddScoped<IUsageEventRepository, UsageEventRepository>();

        s.AddMassTransit(x => x.UsingRabbitMq((ctx, cfg) =>
        {
            cfg.Host(c["RabbitMq:Host"] ?? "Localhost", "/", h =>
            {
                h.Username(c["RabbitMq:User"] ?? "guest");
                h.Password(c["RabbitMq:Password"] ?? "guest");
            });
            cfg.ConfigureEndpoints(ctx);
        }));
        s.AddScoped<IUsageEventPublisher, MassTransitUsageEventPublisher>();
        return s;
    }
}