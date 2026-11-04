namespace Apta.WebUI.Extensions;

[ExcludeFromCodeCoverage(Justification = "does not contain any logic that can be tested.")]
internal static class SettingsExtension
{
    public static ISettings BindSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration
            .GetSection(nameof(Settings))
            .Get<Settings>()!;

        services.AddSingleton<ISettings>(settings);

        return settings;
    }
}
