namespace Apta.Infrastructure.IoC.Extensions;

[ExcludeFromCodeCoverage(Justification = "does not contain any logic that can be tested.")]
internal static class ServicesExtension
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<ICustomerParser, CsvCustomerParser>();
    }
}
