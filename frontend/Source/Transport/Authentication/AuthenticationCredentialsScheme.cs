namespace Apta.WebUI.Transport.Authentication;

public sealed record AuthenticationCredentialsScheme
{
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
}
