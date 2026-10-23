using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TeleBill.Ingestion.Application.Usage;

namespace TeleBill.Ingestion.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection s)
    {
        s.AddValidatorsFromAssemblyContaining<RecordUsageRequestValidator>();
        s.AddScoped<RecordUsageHandler>();
        return s;
    }
}