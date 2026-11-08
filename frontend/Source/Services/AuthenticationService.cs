namespace Apta.WebUI.Services;

public sealed class AuthenticationService(IQueryClient queryClient, IAuthenticationClient authenticationClient, JwtBearerAuthenticationStateProvider authenticationState)
{
    public IMutation<string, bool> LogoutMutation { get; private set; } =
        queryClient.CreateMutation(new MutationOptions<string, bool>
        {
            Mutator = async (refreshToken, cancellation) =>
            {
                var result = await authenticationClient.LogoutAsync(refreshToken, cancellation);

                await authenticationState.LogoutAsync();
                return result.IsSuccess;
            }
        });
}
