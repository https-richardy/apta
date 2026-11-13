namespace Apta.WebApi.Extensions;

[ExcludeFromCodeCoverage(Justification = "contains only sentry monitoring configuration with no business logic.")]
public static class MonitoringExtension
{
    public static void AddMonitoring(this WebApplicationBuilder builder)
    {
        /* prevents sentry from being initialized outside development and production (e.g. testing). */
        if (!builder.Environment.IsDevelopment() && !builder.Environment.IsProduction())
            return;

        var settings = builder.Services.BuildServiceProvider()
            .GetRequiredService<ISettings>();

        /* https://docs.sentry.io/platforms/dotnet/ */
        builder.WebHost.UseSentry(options =>
        {
            options.Dsn = settings.Observability.SentryDsn;
            options.Environment = builder.Environment.EnvironmentName;
            options.TracesSampleRate = 1.0;
            options.EnableLogs = true;
            options.UseSerilog();
        });
    }
}
