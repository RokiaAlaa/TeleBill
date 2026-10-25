using Microsoft.AspNetCore.Mvc;
using TeleBill.Ingestion.Application.Usage;

namespace TeleBill.Ingestion.Api.Controllers;

[ApiController, Route("api/v1/usage")]
public class UsageController(RecordUsageHandler handler, RecordUsageBatchHandler batchHandler) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Record(RecordUsageRequest request, CancellationToken ct)
    {
        var result = await handler.HandleAsync(request, ct);
        return result.IsDuplicate 
        ? Ok(new { duplicate = true }) 
        : Accepted(new { id = result.Id });
    }

    [HttpPost("batch")]
    public async Task<IActionResult> RecordBatch(RecordUsageBatchRequest request, CancellationToken ct)
        => Ok(await batchHandler.HandleAsync(request, ct)); 

}