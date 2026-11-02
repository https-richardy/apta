namespace Apta.Infrastructure.IoC.Configurations;

public sealed record Settings : ISettings
{
    public DatabaseSettings Database { get; init; } = default!;
}
