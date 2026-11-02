namespace Apta.Infrastructure.IoC.Configurations;

public sealed record Settings : ISettings
{
    public AuthenticationSettings Authentication { get; init; } = default!;
    public DatabaseSettings Database { get; init; } = default!;
}
