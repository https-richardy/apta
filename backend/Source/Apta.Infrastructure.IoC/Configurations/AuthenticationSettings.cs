namespace Apta.Infrastructure.IoC.Configurations;

public sealed record AuthenticationSettings
{
    public required string Authority { get; init; } = default!;
    public required string Audience { get; init; } = default!;
}
