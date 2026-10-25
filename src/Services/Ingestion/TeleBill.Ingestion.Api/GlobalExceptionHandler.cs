using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TeleBill.BuildingBlocks.Domain;
using TeleBill.Ingestion.Application.Usage;

namespace TeleBill.Ingestion.Api;

public sealed class GlobalExceptionHandler(IProblemDetailsService problems, ILogger<GlobalExceptionHandler> log) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext ctx, Exception ex, CancellationToken ct)
    {
        var (status, title) = ex switch
        {
            ValidationException => (400, "Validation failed"),
            DomainException => (422, "Business rule violated"),
            DuplicateUsageEventException => (409, "Duplicate usage event"),
            _ => (500, "Unexpected error")
        };
        if (status == 500) log.LogError(ex, "unhandled exception");

        var pd = new ProblemDetails { Status = status, Title = title, Detail = status == 500 ? null : ex.Message};
        if (ex is ValidationException ve)
            pd.Extensions["errors"] = ve.Errors.GroupBy(e => e.PropertyName).ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        ctx.Response.StatusCode = status;
        return await problems.TryWriteAsync(new() { HttpContext= ctx, ProblemDetails = pd, Exception = ex });
    }
}
