namespace Apta.WebUI.Clients.Interfaces;

public interface IAuthenticationClient
{
    public Task<Result<AuthenticationScheme>> AuthenticateAsync(
        AuthenticationCredentialsScheme credentials,
        CancellationToken cancellation = default
    );
}
