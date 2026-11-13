namespace Apta.Tests.Fixtures;

public sealed class WebApplicationFixture(PostgresDatabase database) : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = default!;

    public HttpClient HttpClient { get; private set; } = default!;
    public IServiceProvider Services { get; private set; } = default!;

    public ValueTask InitializeAsync()
    {
        /* database settings pointing at the shared test container */
        Environment.SetEnvironmentVariable("Settings__Database__ConnectionString", database.ConnectionString);
        Environment.SetEnvironmentVariable("Settings__Database__MaxRetryCount", "5");
        Environment.SetEnvironmentVariable("Settings__Database__MaxRetryDelay", "30");

        /* authentication settings required to boot the host */
        Environment.SetEnvironmentVariable("Settings__Authentication__Authority", "https://localhost:8080");
        Environment.SetEnvironmentVariable("Settings__Authentication__Audience", "apta");

        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureTestServices(services =>
            {
                var descriptor = services.SingleOrDefault(descriptor => descriptor.ServiceType == typeof(DbContextOptions<DatabaseContext>));
                if (descriptor is not null)
                    services.Remove(descriptor);

                services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(database.ConnectionString));

                /* replaces bearer authentication with a scheme that authenticates every request */
                var builder = services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = AppDomain.CurrentDomain.FriendlyName;
                    options.DefaultChallengeScheme = AppDomain.CurrentDomain.FriendlyName;
                    options.DefaultScheme = AppDomain.CurrentDomain.FriendlyName;
                });

                builder.AddScheme<AuthenticationSchemeOptions, AuthenticationHandlerFixture>(AppDomain.CurrentDomain.FriendlyName, _ => { });
                services.AddAuthorizationBuilder()
                    .SetDefaultPolicy(new AuthorizationPolicyBuilder(AppDomain.CurrentDomain.FriendlyName)
                    .RequireAuthenticatedUser()
                    .Build());
            });
        });

        HttpClient = _factory.CreateClient();
        Services = _factory.Services;

        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        HttpClient.Dispose();

        await _factory.DisposeAsync();
    }
}
