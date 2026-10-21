namespace TeleBill.BuildingBlocks.Domain;

public abstract class Entity<TId> where TId : notnull
{
    public TId Id { get; protected set; } = default!;
}

public class DomainException(string message) : Exception(message);