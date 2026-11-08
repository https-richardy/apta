namespace Apta.WebUI.Configuration;

public sealed record GatewaySettings
{
    public string Address { get; init; } = default!;
    public string Timeout { get; init; } = default!;
}
