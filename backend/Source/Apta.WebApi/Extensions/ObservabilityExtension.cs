namespace Apta.WebApi.Extensions;

[ExcludeFromCodeCoverage(Justification = "contains only logging pipeline configuration with no business logic.")]
public static class ObservabilityExtension
{
    public static void AddObservability(this WebApplicationBuilder builder)
    {
        /* prevents seq and sentry from being initialized outside development and production (e.g. testing). */
        if (!builder.Environment.IsDevelopment() && !builder.Environment.IsProduction())
            return;

        builder.Host.UseSerilog((context, services, logger) =>
        {
            var settings = services.GetRequiredService<ISettings>();

            logger
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq(settings.Observability.SeqCollector)
                .WriteTo.Sentry();
        });
    }
}
