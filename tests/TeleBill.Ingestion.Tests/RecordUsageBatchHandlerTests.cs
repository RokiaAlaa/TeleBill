using FluentValidation;
using NSubstitute;
using Shouldly;
using TeleBill.Ingestion.Application.Usage;
using TeleBill.Ingestion.Domain;

namespace TeleBill.Ingestion.Tests;

public class RecordUsageBatchHandlerTests
{
    private readonly IUsageEventRepository _repo = Substitute.For<IUsageEventRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly IUsageEventPublisher _publisher = Substitute.For<IUsageEventPublisher>();
    private readonly RecordUsageBatchHandler _sut;

    public RecordUsageBatchHandlerTests()
    {
        _sut = new RecordUsageBatchHandler(
            new RecordUsageRequestValidator(TestData.Time), _repo, _uow, _publisher, TestData.Time
        );
    }

    [Fact]
    public async Task Batch_counts_accepted_duplicates_and_rejected_correctly()
    {
        _repo.GetExistingExternalIdsAsync(Arg.Any<string[]>(), Arg.Any<CancellationToken>()).Returns(new HashSet<string> { "evt-existing" });

        var batch = new RecordUsageBatchRequest([
            TestData.Request("evt-bad", msisdn: "0101234"),
            TestData.Request("evt-1"),
            TestData.Request("evt-2"),
            TestData.Request("evt-1"),
            TestData.Request("evt-existing"),
        ]);

        var result = await _sut.HandleAsync(batch, CancellationToken.None);

        result.Accepted.ShouldBe(2);
        result.Duplicates.ShouldBe(2);
        result.Rejected.Count.ShouldBe(1);
        result.Rejected.First().Index.ShouldBe(0);

        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _publisher.Received(1).PublishBatchAsync(Arg.Is<IReadOnlyCollection<UsageEvent>>(c => c.Count == 2), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Batch_with_only_duplicates_does_not_save_or_publish()
    {
        _repo.GetExistingExternalIdsAsync(Arg.Any<string[]>(), Arg.Any<CancellationToken>()).Returns(new HashSet<string> { "evt-existing" });

        var batch = new RecordUsageBatchRequest([TestData.Request("evt-existing")]);

        var result = await _sut.HandleAsync(batch, CancellationToken.None);

        result.Accepted.ShouldBe(0);
        result.Duplicates.ShouldBe(1);
        await _uow.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        await _publisher.DidNotReceive().PublishBatchAsync(
            Arg.Any<IReadOnlyCollection<UsageEvent>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Batch_over_limit_throws()
    {
        var items = Enumerable.Range(0, RecordUsageBatchHandler.MaxBatchSize + 1)
            .Select(n => TestData.Request($"evt-{n}")).ToList();
        
        await Should.ThrowAsync<ValidationException>(() =>
            _sut.HandleAsync(new RecordUsageBatchRequest(items), CancellationToken.None));
    }
}