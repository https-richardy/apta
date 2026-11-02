namespace Apta.WebApi.Extensions;

[ExcludeFromCodeCoverage(Justification = "does not contain any logic that can be tested.")]
public static class WebInfrastructureExtension
{
    public static void AddWebInfrastructure(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddControllers();
        services.AddHttpContextAccessor();
        services.AddEndpointsApiExplorer();

        services.AddTransformers();
        services.AddAuthorization();
    }
}
