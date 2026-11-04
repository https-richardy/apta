namespace Apta.WebUI.Extensions;

[ExcludeFromCodeCoverage(Justification = "this class is responsible for configuring the dependency injection container and does not contain any logic that can be tested.")]
public static class InfrastructureExtension
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.BindSettings(configuration);
        services.AddBlazoredLocalStorage();
        services.AddFluentUIComponents();
        services.AddBearerAuthentication();
    }
}
