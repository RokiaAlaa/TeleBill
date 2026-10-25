namespace TeleBill.Ingestion.Application.Usage;

public sealed class DuplicateUsageEventException() : Exception("Usage event already exists.");