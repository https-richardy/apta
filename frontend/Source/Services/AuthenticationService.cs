namespace Apta.WebUI.Services;

public sealed class AuthenticationService(IQueryClient queryClient, IAuthenticationClient authenticationClient, JwtBearerAuthenticationStateProvider authenticationState, INotificationService notification, NavigationManager navigation)
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
            },
            OnSuccess = (_, _) =>
            {
                notification.ShowSuccessToastAsync("Autenticação bem-sucedida", "Suas credenciais foram validadas e uma sessão segura foi estabelecida.");
                navigation.NavigateTo("/");
            },
            OnFailure = _ =>
            {
                notification.CloseAllToastsAsync();
                notification.ShowErrorToastAsync("Falha na autenticação", "Verifique se o seu email e a sua senha estão corretos e tente novamente.");
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
            },
            OnSuccess = (_, _) =>
            {
                notification.ShowSuccessToastAsync("Sessão encerrada", "Sua sessão foi encerrada com segurança. Autentique-se novamente para continuar.");
                navigation.NavigateTo("/authentication/protocol/connect/");
            }
        });
}
