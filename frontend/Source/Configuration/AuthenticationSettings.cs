namespace Apta.WebUI.Configuration;

public sealed record AuthenticationSettings
{
    public string Authority { get; init; } = default!;
    public string Audience { get; init; } = default!;
}
