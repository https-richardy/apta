namespace Apta.Tests.Fixtures;

public sealed class AuthenticationHandlerFixture(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) :
    AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim("preferred_username", "administrator@apta.com.br"),
            new Claim("name", "Administrator"),

            /* the following claims are required to satisfy the default authorization policy */
            new Claim(ClaimTypes.Name, "administrator@apta.com.br"),
            new Claim(ClaimTypes.NameIdentifier, "user_d3TjZUiCvu32Uq2LiapbNWjKa")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);

        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name)));
    }
}
