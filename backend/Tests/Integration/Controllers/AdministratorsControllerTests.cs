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

    [Fact(DisplayName = "[e2e] - when PUT /api/v1/administrators/{id} is called with an existing administrator, 200 is returned")]
    public async Task When_UpdateAdministrator_IsCalled_WithExistingAdministrator_Then_200OkIsReturned()
    {
        /* arrange: persist an administrator to be updated */
        var httpClient = factory.HttpClient;
        var administrator = _fixture.Build<Administrator>()
            .With(entity => entity.Code, "58113812")
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .Create();

        await factory.Context.Administrators.AddAsync(administrator, TestContext.Current.CancellationToken);
        await factory.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        /* arrange: create a request with new trade name and code */
        var request = _fixture.Build<UpdateAdministratorScheme>()
            .With(scheme => scheme.Name, "EMBRACON ADMINISTRADORA")
            .With(scheme => scheme.Code, "11122233")
            .Create();

        /* act: send PUT request to the administrators endpoint */
        var response = await httpClient.PutAsJsonAsync($"/api/v1/administrators/{administrator.Id}", request, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        /* assert: verify http response status and content */
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(content));

        /* assert: deserialize response */
        var result = JsonSerializer.Deserialize<AdministratorScheme>(content, _serializer);

        /* assert: validate the updated administrator */
        Assert.NotNull(result);

        Assert.Equal(administrator.Id, result.Id);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Code, result.Code);
    }

    [Fact(DisplayName = "[e2e] - when PUT /api/v1/administrators/{id} is called with a non-existent administrator, 404 is returned")]
    public async Task When_UpdateAdministrator_IsCalled_WithNonExistentAdministrator_Then_404NotFoundIsReturned()
    {
        /* arrange: create a request targeting an administrator that does not exist */
        var httpClient = factory.HttpClient;
        var request = _fixture.Build<UpdateAdministratorScheme>()
            .With(scheme => scheme.Name, "EMBRACON")
            .With(scheme => scheme.Code, "58113812")
            .Create();

        /* act: send PUT request to the administrators endpoint */
        var response = await httpClient.PutAsJsonAsync("/api/v1/administrators/administrator_d3TjZUiCvu22Uf2LkapbNWjKa", request, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        /* assert: verify http response status and content */
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(content));

        /* assert: deserialize response */
        var result = JsonSerializer.Deserialize<Error>(content, _serializer);

        /* assert: validate returned error */
        Assert.NotNull(result);
        Assert.Equal(AdministratorErrors.DoesNotExist, result);
    }

    [Fact(DisplayName = "[e2e] - when PUT /api/v1/administrators/{id} is called with a code taken by another administrator, 409 is returned")]
    public async Task When_UpdateAdministrator_IsCalled_WithCodeTakenByAnotherAdministrator_Then_409ConflictIsReturned()
    {
        /* arrange: persist two administrators with distinct codes */
        var httpClient = factory.HttpClient;
        var administrators = _fixture.Build<Administrator>()
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .CreateMany(2)
            .ToList();

        administrators[0].Code = "58113812";
        administrators[1].Code = "11122233";

        await factory.Context.Administrators.AddRangeAsync(administrators, TestContext.Current.CancellationToken);
        await factory.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        /* arrange: update the first administrator with the code already taken by the second */
        var request = _fixture.Build<UpdateAdministratorScheme>()
            .With(scheme => scheme.Name, "EMBRACON")
            .With(scheme => scheme.Code, "11122233")
            .Create();

        /* act: send PUT request to the administrators endpoint */
        var response = await httpClient.PutAsJsonAsync($"/api/v1/administrators/{administrators[0].Id}", request, TestContext.Current.CancellationToken);
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

    [Fact(DisplayName = "[e2e] - when DELETE /api/v1/administrators/{id} is called with an existing administrator, 204 is returned")]
    public async Task When_DeleteAdministrator_IsCalled_WithExistingAdministrator_Then_204NoContentIsReturned()
    {
        /* arrange: persist an administrator to be deleted */
        var httpClient = factory.HttpClient;
        var administrator = _fixture.Build<Administrator>()
            .With(entity => entity.Code, "58113812")
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .Create();

        await factory.Context.Administrators.AddAsync(administrator, TestContext.Current.CancellationToken);
        await factory.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        /* act: send DELETE request to the administrators endpoint */
        var response = await httpClient.DeleteAsync($"/api/v1/administrators/{administrator.Id}", TestContext.Current.CancellationToken);

        /* assert: verify http response status */
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        /* assert: verify the administrator was soft deleted */
        var repository = factory.Services.GetRequiredService<IAdministratorRepository>();
        var filters = AdministratorFilters.AsBuilder()
            .WithIdentifier(administrator.Id)
            .Build();

        var administrators = await repository.FilterAdministratorsAsync(filters, TestContext.Current.CancellationToken);
        var deletedAdministrator = administrators.FirstOrDefault();

        Assert.NotNull(deletedAdministrator);
        Assert.True(deletedAdministrator.IsDeleted);
    }

    [Fact(DisplayName = "[e2e] - when DELETE /api/v1/administrators/{id} is called with a non-existent administrator, 404 is returned")]
    public async Task When_DeleteAdministrator_IsCalled_WithNonExistentAdministrator_Then_404NotFoundIsReturned()
    {
        /* arrange: resolves http client instance from integration environment */
        var httpClient = factory.HttpClient;

        /* act: send DELETE request targeting an administrator that does not exist */
        var response = await httpClient.DeleteAsync("/api/v1/administrators/administrator_d3TjZUiCvu22Uf2LkapbNWjKa", TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        /* assert: verify http response status and content */
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(content));

        /* assert: deserialize response */
        var result = JsonSerializer.Deserialize<Error>(content, _serializer);

        /* assert: validate returned error */
        Assert.NotNull(result);
        Assert.Equal(AdministratorErrors.DoesNotExist, result);
    }

    public async ValueTask DisposeAsync() => await ValueTask.FromResult(ValueTask.CompletedTask);
    public async ValueTask InitializeAsync() => await factory.CleanDatabaseAsync();
}
