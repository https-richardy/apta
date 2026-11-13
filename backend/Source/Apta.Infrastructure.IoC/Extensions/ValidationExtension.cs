namespace Apta.Infrastructure.IoC.Extensions;

[ExcludeFromCodeCoverage(Justification = "does not contain any logic that can be tested.")]
internal static class ValidationExtension
{
    public static void AddValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<AdministratorCreationScheme>, AdministratorCreationSchemeValidator>();
        services.AddScoped<IValidator<AdministratorFiltersParameters>, AdministratorFiltersParametersValidator>();
        services.AddScoped<IValidator<UpdateAdministratorScheme>, UpdateAdministratorSchemeValidator>();

        services.AddScoped<IValidator<CustomerCreationScheme>, CustomerCreationSchemeValidator>();
        services.AddScoped<IValidator<CustomerFiltersParameters>, CustomerFiltersParametersValidator>();
    }
}