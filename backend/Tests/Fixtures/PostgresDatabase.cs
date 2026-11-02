namespace Apta.Tests.Fixtures;

public sealed class PostgresDatabase : IAsyncLifetime
{
    public string ConnectionString { get; private set; } = default!;
    public DatabaseContext Context { get; private set; } = default!;

    // build a container with the official postgres image
    // see more: https://dotnet.testcontainers.org/modules/postgres/

    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:17-alpine")
        .WithDatabase("apta")
        .WithUsername("apta")
        .WithPassword("apta")
        .WithCleanUp(true)
        .Build();

    public async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
        await _container.DisposeAsync();
    }

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();

        ConnectionString = _container.GetConnectionString();

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        Context = new DatabaseContext(options);

        await Context.Database.EnsureCreatedAsync();
    }

    public async Task CleanDatabaseAsync()
    {
        Context.ChangeTracker.Clear();

        await Context.Administrators.ExecuteDeleteAsync();
        await Context.Customers.ExecuteDeleteAsync();
    }
}