namespace Apta.Infrastructure.IoC.Extensions;

[ExcludeFromCodeCoverage(Justification = "does not contain any logic that can be tested.")]
internal static class DispatcherExtension
{
    public static void AddMediator(this IServiceCollection services)
    {
        services.AddDispatcher(configuration =>
        {
            configuration.ScanAssembly<AdministratorCreationHandler>();
        });
    }
}
