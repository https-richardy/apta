namespace Apta.WebUI.Clients.Interfaces;

public interface IAuthenticationClient
{
    public Task<Result<AuthenticationScheme>> AuthenticateAsync(
        AuthenticationCredentialsScheme credentials,
        CancellationToken cancellation = default
    );

    public Task<Result> LogoutAsync(
        String refreshToken,
        CancellationToken cancellation = default
    );
}
