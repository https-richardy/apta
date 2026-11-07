namespace Apta.WebUI.Configuration;

public sealed class Settings : ISettings
{
    public AuthenticationSettings Authentication { get; init; } = default!;
    public GatewaySettings Gateway { get; init; } = default!;
}
