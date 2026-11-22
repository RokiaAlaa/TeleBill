using TeleBill.BuildingBlocks.Domain;

namespace TeleBill.Billing.Domain;

public sealed class Subscriber : Entity<Guid>
{
    private Subscriber() { }
    public string Msisdn {get; private set;} = default!;
    public string FullName {get; private set;} = default!;
    public DateTimeOffset CreatedAt {get; private set;} = default!;

    public static Subscriber Create(string msisdn, string fullName, DateTimeOffset now)
    {
        if(string.IsNullOrWhiteSpace(msisdn))
            throw new DomainException("Msisdn is required.");
        if(string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Full Name is required.");

        return new Subscriber { Id = Guid.CreateVersion7(), Msisdn = msisdn, FullName = fullName, CreatedAt = now };
    }
}