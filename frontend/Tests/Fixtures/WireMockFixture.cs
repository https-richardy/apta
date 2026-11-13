namespace Apta.WebUI.Tests.Fixtures;

public sealed class WireMockFixture : IAsyncLifetime
{
    public HttpClient HttpClient { get; private set; } = default!;
    private WireMockServer _server = default!;
    public WireMockServer Server => _server;

    public ValueTask InitializeAsync()
    {
        _server = WireMockServer.Start();
        HttpClient = new HttpClient { BaseAddress = new Uri(_server.Url!) };

        return ValueTask.CompletedTask;
    }

    public void Reset() => _server.Reset();
    public ValueTask DisposeAsync()
    {
        HttpClient.Dispose();
        _server.Stop();

        return ValueTask.CompletedTask;
    }
}
