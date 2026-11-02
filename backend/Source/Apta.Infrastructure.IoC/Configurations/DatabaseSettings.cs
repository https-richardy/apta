namespace Apta.Infrastructure.IoC.Configurations;

public sealed record DatabaseSettings
{
    public string ConnectionString { get; init; } = default!;

    /* https://learn.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency */
    public int MaxRetryCount { get; init; } = default!;
    public int MaxRetryDelay { get; init; } = default!;
}