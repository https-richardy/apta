namespace Apta.WebUI.Extensions;

[ExcludeFromCodeCoverage(Justification = "does not contain any logic that can be tested.")]
internal static class HttpClientsExtension
{
    public static void AddClients(this IServiceCollection services, ISettings settings)
    {
        services.AddScoped<HttpErrorInterceptor>();
        services.AddScoped<AuthenticationInterceptor>();

        var authenticationClient = services.AddHttpClient<IAuthenticationClient, AuthenticationClient>(client =>
        {
            client.BaseAddress = new Uri(settings.Authentication.Authority);
        });

        authenticationClient.AddHttpMessageHandler<HttpErrorInterceptor>();
        authenticationClient.AddHttpMessageHandler<AuthenticationInterceptor>();
    }
}
