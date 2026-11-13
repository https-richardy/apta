namespace Apta.WebUI.Tests.Fixtures;

public sealed class QueryClientFixture : IAsyncLifetime
{
    private ServiceProvider _provider = default!;
    private IServiceScope _scope = default!;
    public IQueryClient QueryClient { get; private set; } = default!;

    public ValueTask InitializeAsync()
    {
        var services = new ServiceCollection();

        services.AddDotNetQuery(options =>
        {
            options.ExecutionMode = QueryExecutionMode.Csr;
            options.StaleTime = TimeSpan.Zero;
            options.CacheTime = TimeSpan.FromMinutes(5);
        });

        _provider = services.BuildServiceProvider();
        _scope = _provider.CreateScope();

        QueryClient = _scope.ServiceProvider.GetRequiredService<IQueryClient>();

        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        _scope.Dispose();
        await _provider.DisposeAsync();
    }
}
