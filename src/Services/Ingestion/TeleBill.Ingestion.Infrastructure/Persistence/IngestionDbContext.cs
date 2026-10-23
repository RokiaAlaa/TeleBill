using Microsoft.EntityFrameworkCore;
using TeleBill.Ingestion.Application.Usage;
using TeleBill.Ingestion.Domain;

namespace TeleBill.Ingestion.Infrastructure.Persistence;

public sealed class IngestionDbContext(DbContextOptions<IngestionDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<UsageEvent> UsageEvents => Set<UsageEvent>();

    protected override void OnModelCreating(ModelBuilder mb) =>
        mb.ApplyConfigurationsFromAssembly(typeof(IngestionDbContext).Assembly);
}