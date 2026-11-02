namespace Apta.Infrastructure.IoC.Extensions;

[ExcludeFromCodeCoverage(Justification = "does not contain any logic that can be tested.")]
internal static class MappingExtension
{
    public static void AddMapping(this IServiceCollection services)
    {
        Action<IMapperConfigurationExpression> options = mapper =>
        {
            mapper.AddMaps(typeof(AdministratorMapping).Assembly);
        };

        services.AddSingleton(provider =>
        {
            return new MapperConfiguration(options);
        });

        services.AddSingleton<IMapper>(provider =>
        {
            var configuration = provider.GetRequiredService<MapperConfiguration>();

            // mapping definitions are validated once at startup and reused throughout
            // https://docs.automapper.io/en/stable/Dependency-injection.html

            return configuration.CreateMapper();
        });
    }
}