using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TeleBill.Ingestion.Infrastructure.Persistence;

namespace TeleBill.Ingestion.Infrastructure;


public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection s, IConfiguration c)
    {
        s.AddDbContext<IngestionDbContext>(o => o.UseNpgsql(c.GetConnectionString("Default")));
        return s;
    }
}