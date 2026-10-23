using FluentValidation;

namespace TeleBill.Ingestion.Application.Usage;

public sealed class RecordUsageRequestValidator : AbstractValidator<RecordUsageRequest>
{
    public RecordUsageRequestValidator(TimeProvider time)
    {
        RuleFor(x => x.ExternalEventId).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Msisdn).Matches(@"^\+[1-9]\d{7,14}$").WithMessage("MSISDN must be E.164");
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.OccurredAt).Must(d => d <= time.GetUtcNow().AddMinutes(5)).WithMessage("OccurredAt cannot be in the future.");

    }
}