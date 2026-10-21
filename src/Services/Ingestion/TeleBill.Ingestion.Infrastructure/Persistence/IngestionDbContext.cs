using Microsoft.EntityFrameworkCore;
using TeleBill.Ingestion.Domain;

namespace TeleBill.Ingestion.Infrastructure.Persistence;

public sealed class IngestionDbContext(DbContextOptions<IngestionDbContext> options) : DbContext(options)
{
    public DbSet<UsageEvent> UsageEvent => Set<UsageEvent>();

    protected override void OnModelCreating(ModelBuilder mb) =>
        mb.ApplyConfigurationsFromAssembly(typeof(IngestionDbContext).Assembly);
}