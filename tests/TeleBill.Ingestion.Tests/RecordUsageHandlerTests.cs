using FluentValidation;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using TeleBill.Ingestion.Application.Usage;
using TeleBill.Ingestion.Domain;

namespace TeleBill.Ingestion.Tests;

public class RecordUsageHandlerTests
{
    private readonly IUsageEventRepository _repo = Substitute.For<IUsageEventRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly IUsageEventPublisher _publisher = Substitute.For<IUsageEventPublisher>();
    private readonly RecordUsageHandler _sut;

    public RecordUsageHandlerTests()
    {
        _sut = new RecordUsageHandler(
            new RecordUsageRequestValidator(TestData.Time), _repo, _uow, _publisher, TestData.Time
        );
    }

    [Fact]
    public async Task Duplicate_event_is_not_saved_or_published()
    {
        _repo.ExistsAsync("evt-1", Arg.Any<CancellationToken>()).Returns(true);

        var result = await _sut.HandleAsync(TestData.Request("evt-1"), CancellationToken.None);

        result.IsDuplicate.ShouldBeTrue();
        result.Id.ShouldBeNull();
        await _uow.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        await _publisher.DidNotReceive().PublishAsync(Arg.Any<UsageEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Race_condition_duplicate_is_treated_as_duplicate_and_not_published()
    {
        _repo.ExistsAsync("evt-3", Arg.Any<CancellationToken>()).Returns(false);
        _uow.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns<Task<int>>(_ => throw new DuplicateUsageEventException());

        var result = await _sut.HandleAsync(TestData.Request("evt-3"), CancellationToken.None);

        result.IsDuplicate.ShouldBeTrue();
        await _publisher.DidNotReceive().PublishAsync(Arg.Any<UsageEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Invalid_request_throws_validation_exception_and_touches_nothing()
    {
        await Should.ThrowAsync<ValidationException>(() =>
            _sut.HandleAsync(TestData.Request(msisdn: "bad"), CancellationToken.None));

        await _uow.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        await _publisher.DidNotReceive().PublishAsync(Arg.Any<UsageEvent>(), Arg.Any<CancellationToken>());
    }
}