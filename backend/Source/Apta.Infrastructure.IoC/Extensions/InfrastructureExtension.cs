namespace Apta.Infrastructure.IoC.Extensions;

[ExcludeFromCodeCoverage(Justification = "does not contain any logic that can be tested.")]
internal static class InfrastructureExtension
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = services.BindSettings(configuration);

        services.AddDataPersistence(settings);
    }
}
