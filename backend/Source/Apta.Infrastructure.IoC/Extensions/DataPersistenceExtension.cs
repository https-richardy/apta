namespace Apta.Infrastructure.IoC.Extensions;

[ExcludeFromCodeCoverage(Justification = "does not contain any logic that can be tested.")]
internal static class DataPersistenceExtension
{
    public static void AddDataPersistence(this IServiceCollection services, ISettings settings)
    {
        services.AddDbContext<DatabaseContext>(options =>
        {
            options.UseNpgsql(settings.Database.ConnectionString, pgsqlOptions =>
            {
                /* https://learn.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency */
                pgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: settings.Database.MaxRetryCount,
                    maxRetryDelay: TimeSpan.FromSeconds(settings.Database.MaxRetryDelay),

                    /* https://www.postgresql.org/docs/current/errcodes-appendix.html */
                    errorCodesToAdd: 
                    [
                        "40001", /* serialization failure */
                        "40P01", /* deadlock detected */
                        "55P03", /* lock not available */
                        "57014"  /* query canceled */
                    ]);
            });
        });
    }
}
