using MassTransit;
using TeleBill.Billing.Application.Rating;
using TeleBill.BuildingBlocks.Contracts;

namespace TeleBill.Billing.Infrastructure.Messaging;

public sealed class UsageRecordedConsumer(RateUsageHandler handler) : IConsumer<UsageRecorded>
{
    public Task Consume(ConsumeContext<UsageRecorded> ctx) => 
        handler.HandleAsync(ctx.Message, ctx.CancellationToken);
}