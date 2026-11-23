using Microsoft.Extensions.DependencyInjection;
using TeleBill.Billing.Application.Rating;

namespace TeleBill.Billing.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection s)
    {
        s.AddScoped<RateUsageHandler>();
        return s;
    }
}