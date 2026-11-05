namespace Apta.WebUI.Transport.Authentication;

public sealed record AuthenticationCredentialsScheme
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}
