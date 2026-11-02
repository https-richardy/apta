namespace Apta.WebUI.Extensions;

[ExcludeFromCodeCoverage(Justification = "does not contain any logic that can be tested.")]
internal static class AuthenticationExtension
{
    public static void AddBearerAuthentication(this IServiceCollection services)
    {
        services.AddAuthorizationCore();
        services.AddCascadingAuthenticationState();

        services.AddScoped<JwtBearerAuthenticationStateProvider>();
        services.AddScoped<AuthenticationInterceptor>();
        services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<JwtBearerAuthenticationStateProvider>());
    }
}
