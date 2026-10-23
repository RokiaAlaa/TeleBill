using Microsoft.AspNetCore.Mvc;
using TeleBill.Ingestion.Application.Usage;

namespace TeleBill.Ingestion.Api.Controllers;

[ApiController, Route("api/v1/usage")]
public class UsageController(RecordUsageHandler handler) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Record(RecordUsageRequest request, CancellationToken ct)
        => Accepted(new { id = await handler.HandleAsync(request, ct) });
}