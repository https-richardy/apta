namespace Apta.WebUI.Extensions;

[ExcludeFromCodeCoverage(Justification = "does not contain any logic that can be tested.")]
internal static class ValidationExtension
{
    public static void AddValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<AuthenticationCredentialsScheme>, AuthenticationCredentialsValidator>();
        services.AddScoped<IValidator<AdministratorCreationScheme>, AdministratorCreationValidator>();
    }
}
