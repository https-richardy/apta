namespace Apta.Tests.Fixtures;

public sealed class ApplicationFixture : IAsyncLifetime
{
    private readonly PostgresDatabase _database;
    private readonly WebApplicationFixture _factory;

    public HttpClient HttpClient => _factory.HttpClient;
    public IServiceProvider Services => _factory.Services;
    public DatabaseContext Context => _database.Context;

    public ApplicationFixture()
    {
        _database = new PostgresDatabase();
        _factory = new WebApplicationFixture(_database);
    }

    public async ValueTask InitializeAsync()
    {
        await _database.InitializeAsync();
        await _database.CleanDatabaseAsync();
        await _factory.InitializeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _factory.DisposeAsync();
        await _database.DisposeAsync();
    }

    public async Task CleanDatabaseAsync() => await _database.CleanDatabaseAsync();
}
