namespace Apta.WebUI.Services;

public sealed class AuthenticationService(IQueryClient queryClient, IAuthenticationClient authenticationClient, JwtBearerAuthenticationStateProvider authenticationState)
{
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
