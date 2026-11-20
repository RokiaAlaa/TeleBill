using MassTransit;
using Microsoft.Extensions.Logging;
using TeleBill.BuildingBlocks.Contracts;

namespace TeleBill.Billing.Infrastructure.Messaging;

public sealed class UsageRecordedConsumer(ILogger<UsageRecordedConsumer> log) : IConsumer<UsageRecorded>
{
    public Task Consume(ConsumeContext<UsageRecorded> ctx)
    {
        var m = ctx.Message;
        if(m.Msisdn.EndsWith("0000"))
            throw new InvalidOperationException("Simulated failure");
        log.LogInformation("Usage {Id}: {Type} {Qty} for {Msisdn}",
        m.UsageEventId, m.Type, m.Quantity, m.Msisdn);
        return Task.CompletedTask;
    }
}