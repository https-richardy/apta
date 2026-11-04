namespace Apta.WebUI.Transport.Authentication;

public sealed record AuthenticationScheme
{
    public string AccessToken { get; init; } = default!;
    public string RefreshToken { get; init; } = default!;
}
