namespace Apta.WebUI.Extensions;

[ExcludeFromCodeCoverage(Justification = "this class is responsible for configuring the dependency injection container and does not contain any logic that can be tested.")]
internal static class ServicesExtension
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<AdministratorsService>();
    }
}
