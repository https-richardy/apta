namespace Apta.Tests.Integration.Controllers;

[Collection(nameof(DisableParallelizationCollection))]
public sealed class CustomersControllerTests(ApplicationFixture factory) :
    IClassFixture<ApplicationFixture>,
    IAsyncLifetime
{
    private readonly Fixture _fixture = new();
    private readonly JsonSerializerOptions _serializer = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    [Fact(DisplayName = "[e2e] - when POST /api/v1/customers is called with a valid request, 201 is returned")]
    public async Task When_CreateCustomer_IsCalled_WithValidRequest_Then_201CreatedIsReturned()
    {
        /* arrange: resolves http client instance from integration environment */
        var httpClient = factory.HttpClient;
        var request = _fixture.Build<CustomerCreationScheme>()
            .With(customer => customer.Name, "João da Silva")
            .With(customer => customer.Phone, "(11) 98888-7777")
            .With(customer => customer.Document, "123.456.789-09")
            .Create();

        /* act: send POST request to the customers endpoint */
        var response = await httpClient.PostAsJsonAsync("/api/v1/customers", request, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        /* assert: verify http response status and content */
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(content));

        /* assert: deserialize response */
        var result = JsonSerializer.Deserialize<CustomerScheme>(content, _serializer);

        /* assert: validate the returned customer with sanitized contact data */
        Assert.NotNull(result);
        Assert.Equal(request.Name, result.Name);

        Assert.Equal("11988887777", result.Phone);
        Assert.Equal("12345678909", result.Document);

        /* assert: verify the customer was persisted with the sanitized document */
        var repository = factory.Services.GetRequiredService<ICustomerRepository>();
        var filters = CustomerFilters.AsBuilder()
            .WithDocument("12345678909")
            .Build();

        var customers = await repository.FilterCustomersAsync(filters, TestContext.Current.CancellationToken);
        var customer = customers.FirstOrDefault();

        Assert.NotNull(customer);
        Assert.Equal(result.Id, customer.Id);
    }

    [Fact(DisplayName = "[e2e] - when POST /api/v1/customers is called without a document, 201 is returned and the response falls back to the unspecified marker")]
    public async Task When_CreateCustomer_IsCalled_WithoutDocument_Then_201CreatedIsReturned()
    {
        /* arrange: create a request with only the name, as a lead without contact data */
        var httpClient = factory.HttpClient;
        var request = _fixture.Build<CustomerCreationScheme>()
            .With(customer => customer.Name, "João da Silva")
            .Without(customer => customer.Phone)
            .Without(customer => customer.Document)
            .Create();

        /* act: send POST request to the customers endpoint */
        var response = await httpClient.PostAsJsonAsync("/api/v1/customers", request, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        /* assert: verify http response status and content */
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(content));

        /* assert: deserialize response */
        var result = JsonSerializer.Deserialize<CustomerScheme>(content, _serializer);

        /* assert: validate the returned customer falls back to the unspecified marker */
        Assert.NotNull(result);
        Assert.Equal(request.Name, result.Name);

        Assert.Equal("NÃO ESPECIFICADO", result.Phone);
        Assert.Equal("NÃO ESPECIFICADO", result.Document);
    }

    [Fact(DisplayName = "[e2e] - when POST /api/v1/customers is called with an existing document, 409 is returned")]
    public async Task When_CreateCustomer_IsCalled_WithExistingDocument_Then_409ConflictIsReturned()
    {
        /* arrange: persist an existing customer with a known document */
        var httpClient = factory.HttpClient;
        var customer = _fixture.Build<Customer>()
            .With(entity => entity.Phone, "11988887777")
            .With(entity => entity.Document, "12345678909")
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .Create();

        await factory.Context.Customers.AddAsync(customer, TestContext.Current.CancellationToken);
        await factory.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        /* arrange: create a request with the same document */
        var request = _fixture.Build<CustomerCreationScheme>()
            .With(scheme => scheme.Name, "João da Silva")
            .With(scheme => scheme.Phone, "(11) 98888-7777")
            .With(scheme => scheme.Document, "123.456.789-09")
            .Create();

        /* act: send POST request to the customers endpoint */
        var response = await httpClient.PostAsJsonAsync("/api/v1/customers", request, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        /* assert: verify http response status and content */
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(content));

        /* assert: deserialize response */
        var result = JsonSerializer.Deserialize<Error>(content, _serializer);

        /* assert: validate returned error */
        Assert.NotNull(result);
        Assert.Equal(CustomerErrors.AlreadyExists, result);
    }

    [Fact(DisplayName = "[e2e] - when GET /api/v1/customers is called and there are customers, 200 is returned with the customers")]
    public async Task When_GetCustomers_IsCalled_AndThereAreCustomers_Then_200OkIsReturnedWithTheCustomers()
    {
        /* arrange: persist a set of customers with distinct documents */
        var httpClient = factory.HttpClient;
        var customers = _fixture.Build<Customer>()
            .With(entity => entity.Phone, "11988887777")
            .With(entity => entity.Document, () => Guid.NewGuid().ToString("N")[..11])
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .CreateMany(3)
            .ToList();

        await factory.Context.Customers.AddRangeAsync(customers, TestContext.Current.CancellationToken);
        await factory.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        /* act: send GET request to the customers endpoint */
        var response = await httpClient.GetAsync("/api/v1/customers", TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        /* assert: verify http response status and content */
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(content));

        /* assert: deserialize response and validate data structure */
        var result = JsonSerializer.Deserialize<Pagination<CustomerScheme>>(content, _serializer);

        /* assert: ensure the response contains the persisted customers */
        Assert.NotNull(result);
        Assert.NotEmpty(result.Items);

        /* assert: check if the number of returned customers matches the inserted ones */
        Assert.Equal(customers.Count, result.Items.Count);
        Assert.All(customers, customer => Assert.Contains(result.Items, item => item.Document == customer.Document));
    }

    [Fact(DisplayName = "[e2e] - when POST /api/v1/customers is called with an invalid document, 400 is returned")]
    public async Task When_CreateCustomer_IsCalled_WithInvalidDocument_Then_400BadRequestIsReturned()
    {
        /* arrange: create a request carrying an invalid cpf */
        var httpClient = factory.HttpClient;
        var request = _fixture.Build<CustomerCreationScheme>()
            .With(customer => customer.Name, "João da Silva")
            .With(customer => customer.Phone, "(11) 98888-7777")
            .With(customer => customer.Document, "123")
            .Create();

        /* act: send POST request to the customers endpoint */
        var response = await httpClient.PostAsJsonAsync("/api/v1/customers", request, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        /* assert: verify http response status and content */
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(content));

        /* assert: verify the validation failure mentions the invalid cpf */
        Assert.Contains("cpf", content, StringComparison.OrdinalIgnoreCase);
    }

    public async ValueTask DisposeAsync() => await ValueTask.FromResult(ValueTask.CompletedTask);
    public async ValueTask InitializeAsync() => await factory.CleanDatabaseAsync();
}
