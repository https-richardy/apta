namespace Apta.WebUI.Services;

public sealed class AuthenticationService(IQueryClient queryClient, IAuthenticationClient authenticationClient, JwtBearerAuthenticationStateProvider authenticationState)
{
    public IMutation<AuthenticationCredentialsScheme, AuthenticationScheme> AuthenticationMutation { get; private set; } =
        queryClient.CreateMutation(new MutationOptions<AuthenticationCredentialsScheme, AuthenticationScheme>
        {
            Mutator = async (credentials, cancellation) =>
            {
                var result = await authenticationClient.AuthenticateAsync(credentials, cancellation);
                if (result.IsFailure || result.Data is null)
                {
                    throw new InvalidOperationException("não foi possível autenticar o usuário");
                }

                await authenticationState.AuthenticateAsync(result.Data.AccessToken, result.Data.RefreshToken);
                return result.Data;
            }
        });

    public IMutation<String, Result> LogoutMutation { get; private set; } =
        queryClient.CreateMutation(new MutationOptions<String, Result>
        {
            Mutator = async (refreshToken, cancellation) =>
            {
                var result = await authenticationClient.LogoutAsync(refreshToken, cancellation);

                await authenticationState.LogoutAsync();
                return result;
            }
        });
}
