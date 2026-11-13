namespace Apta.Tests.Integration.Controllers;

[Collection(nameof(DisableParallelizationCollection))]
public sealed class AdministratorsControllerTests(ApplicationFixture factory) :
    IClassFixture<ApplicationFixture>,
    IAsyncLifetime
{
    private readonly Fixture _fixture = new();
    private readonly JsonSerializerOptions _serializer = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    [Fact(DisplayName = "[e2e] - when POST /api/v1/administrators is called with a valid request, 201 is returned")]
    public async Task When_CreateAdministrator_IsCalled_WithValidRequest_Then_201CreatedIsReturned()
    {
        /* arrange: resolves http client instance from integration environment */
        var httpClient = factory.HttpClient;
        var request = _fixture.Build<AdministratorCreationScheme>()
            .With(administrator => administrator.Name, "EMBRACON")
            .With(administrator => administrator.Code, "58113812")
            .Create();

        /* act: send POST request to the administrators endpoint */
        var response = await httpClient.PostAsJsonAsync("/api/v1/administrators", request, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        /* assert: verify http response status and content */
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(content));

        /* assert: deserialize response */
        var result = JsonSerializer.Deserialize<AdministratorScheme>(content, _serializer);

        /* assert: validate returned administrator */
        Assert.NotNull(result);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Code, result.Code);

        /* assert: verify administrator was persisted */
        var repository = factory.Services.GetRequiredService<IAdministratorRepository>();
        var filters = AdministratorFilters.AsBuilder()
            .WithCode(request.Code)
            .Build();

        var administrators = await repository.FilterAdministratorsAsync(filters, TestContext.Current.CancellationToken);
        var administrator = administrators.FirstOrDefault();

        Assert.NotNull(administrator);
        Assert.Equal(result.Id, administrator.Id);
    }

    [Fact(DisplayName = "[e2e] - when POST /api/v1/administrators is called with an existing code, 409 is returned")]
    public async Task When_CreateAdministrator_IsCalled_WithExistingCode_Then_409ConflictIsReturned()
    {
        /* arrange: persist an existing administrator with a known code */
        var httpClient = factory.HttpClient;
        var administrator = _fixture.Build<Administrator>()
            .With(entity => entity.Code, "58113812")
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .Create();

        await factory.Context.Administrators.AddAsync(administrator, TestContext.Current.CancellationToken);
        await factory.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        /* arrange: create a request with the same administrator code */
        var request = _fixture.Build<AdministratorCreationScheme>()
            .With(scheme => scheme.Name, "EMBRACON")
            .With(scheme => scheme.Code, "58113812")
            .Create();

        /* act: send POST request to the administrators endpoint */
        var response = await httpClient.PostAsJsonAsync("/api/v1/administrators", request, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        /* assert: verify http response status and content */
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(content));

        /* assert: deserialize response */
        var result = JsonSerializer.Deserialize<Error>(content, _serializer);

        /* assert: validate returned error */
        Assert.NotNull(result);
        Assert.Equal(AdministratorErrors.AlreadyExists, result);
    }

    [Fact(DisplayName = "[e2e] - when GET /api/v1/administrators is called and there are administrators, 200 is returned with the administrators")]
    public async Task When_GetAdministrators_IsCalled_AndThereAreAdministrators_Then_200OkIsReturnedWithTheAdministrators()
    {
        /* arrange: resolves http client instance from integration environment */
        var httpClient = factory.HttpClient;
        var administrators = _fixture.Build<Administrator>()
            .With(entity => entity.Code, () => Guid.NewGuid().ToString("N")[..8])
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .CreateMany(3)
            .ToList();

        await factory.Context.Administrators.AddRangeAsync(administrators, TestContext.Current.CancellationToken);
        await factory.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        /* act: send GET request to the administrators endpoint */
        var response = await httpClient.GetAsync("/api/v1/administrators", TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        /* assert: verify http response status and content */
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(content));

        /* assert: deserialize response and validate data structure */
        var result = JsonSerializer.Deserialize<Pagination<AdministratorScheme>>(content, _serializer);

        /* assert: ensure response contains administrators */
        Assert.NotNull(result);
        Assert.NotEmpty(result.Items);

        /* assert: check if the number of returned administrators matches the inserted ones */
        Assert.Equal(administrators.Count, result.Items.Count);
        Assert.All(administrators, administrator =>
        {
            Assert.Contains(result.Items, item => item.Code == administrator.Code);
            Assert.Contains(result.Items, item => item.Name == administrator.Name);
        });
    }

    public async ValueTask DisposeAsync() => await ValueTask.FromResult(ValueTask.CompletedTask);
    public async ValueTask InitializeAsync() => await factory.CleanDatabaseAsync();
}
