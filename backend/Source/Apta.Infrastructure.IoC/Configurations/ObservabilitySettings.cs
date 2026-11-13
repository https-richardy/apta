namespace Apta.Infrastructure.IoC.Configurations;

public sealed record ObservabilitySettings
{
    public string SentryDsn { get; init; } = default!;
    public string SeqCollector { get; init; } = default!;
}
