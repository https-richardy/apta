namespace Apta.Infrastructure.IoC.Extensions;

[ExcludeFromCodeCoverage(Justification = "does not contain any logic that can be tested.")]
public static class QueueExtension
{
    public static void AddQueue(this IServiceCollection services)
    {
        services.AddMemoryQueue(options =>
        {
            options.ScanAssembly<AdministratorCreationHandler>();
        });
    }
}
