using Microsoft.EntityFrameworkCore;
using TeleBill.Billing.Application;
using TeleBill.Billing.Infrastructure;
using TeleBill.Billing.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<BillingDbContext>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db, TimeProvider.System.GetUtcNow());
}

app.MapControllers();

app.Run();