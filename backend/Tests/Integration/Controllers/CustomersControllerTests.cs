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
            .With(customer => customer.Phone, "11988887777")
            .With(customer => customer.Document, "12345678909")
            .With(customer => customer.CreatedAt, DateTime.UtcNow)
            .With(customer => customer.UpdatedAt, DateTime.UtcNow)
            .With(customer => customer.IsDeleted, false)
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
            .With(customer => customer.Phone, "11988887777")
            .With(customer => customer.Document, () => Guid.NewGuid().ToString("N")[..11])
            .With(customer => customer.CreatedAt, DateTime.UtcNow)
            .With(customer => customer.UpdatedAt, DateTime.UtcNow)
            .With(customer => customer.IsDeleted, false)
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
    }

    [Fact(DisplayName = "[e2e] - when PUT /api/v1/customers/{id} is called with an existing customer, 200 is returned")]
    public async Task When_UpdateCustomer_IsCalled_WithExistingCustomer_Then_200OkIsReturned()
    {
        /* arrange: persist a customer to be updated */
        var httpClient = factory.HttpClient;
        var customer = _fixture.Build<Customer>()
            .With(customer => customer.Phone, "11988887777")
            .With(customer => customer.Document, "12345678909")
            .With(customer => customer.CreatedAt, DateTime.UtcNow)
            .With(customer => customer.UpdatedAt, DateTime.UtcNow)
            .With(customer => customer.IsDeleted, false)
            .Create();

        await factory.Context.Customers.AddAsync(customer, TestContext.Current.CancellationToken);
        await factory.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        /* arrange: create a request with new data */
        var request = _fixture.Build<UpdateCustomerScheme>()
            .With(scheme => scheme.Name, "João da Silva")
            .With(scheme => scheme.Phone, "(11) 97777-6666")
            .With(scheme => scheme.Document, "987.654.321-00")
            .Create();

        /* act: send PUT request to the customers endpoint */
        var response = await httpClient.PutAsJsonAsync($"/api/v1/customers/{customer.Id}", request, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        /* assert: verify http response status and content */
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(content));

        /* assert: deserialize response */
        var result = JsonSerializer.Deserialize<CustomerScheme>(content, _serializer);

        /* assert: validate the updated customer with sanitized contact data */
        Assert.NotNull(result);
        Assert.Equal(customer.Id, result.Id);

        Assert.Equal(request.Name, result.Name);
        Assert.Equal("11977776666", result.Phone);
        Assert.Equal("98765432100", result.Document);
    }

    [Fact(DisplayName = "[e2e] - when PUT /api/v1/customers/{id} is called with a non-existent customer, 404 is returned")]
    public async Task When_UpdateCustomer_IsCalled_WithNonExistentCustomer_Then_404NotFoundIsReturned()
    {
        /* arrange: create a request targeting a customer that does not exist */
        var httpClient = factory.HttpClient;
        var request = _fixture.Build<UpdateCustomerScheme>()
            .With(scheme => scheme.Name, "João da Silva")
            .With(scheme => scheme.Phone, "(11) 98888-7777")
            .With(scheme => scheme.Document, "123.456.789-09")
            .Create();

        /* act: send PUT request to the customers endpoint */
        var response = await httpClient.PutAsJsonAsync("/api/v1/customers/customer_d3TjZUiCvu22Uf2LkapbNWjKa", request, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        /* assert: verify http response status and content */
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(content));

        /* assert: deserialize response */
        var result = JsonSerializer.Deserialize<Error>(content, _serializer);

        /* assert: validate returned error */
        Assert.NotNull(result);
        Assert.Equal(CustomerErrors.DoesNotExist, result);
    }

    [Fact(DisplayName = "[e2e] - when PUT /api/v1/customers/{id} is called with a document taken by another customer, 409 is returned")]
    public async Task When_UpdateCustomer_IsCalled_WithDocumentTakenByAnotherCustomer_Then_409ConflictIsReturned()
    {
        /* arrange: persist the customer to be updated and another one already holding the target document */
        var httpClient = factory.HttpClient;
        var customer = _fixture.Build<Customer>()
            .With(customer => customer.Phone, "11988887777")
            .With(customer => customer.Document, "12345678909")
            .With(customer => customer.CreatedAt, DateTime.UtcNow)
            .With(customer => customer.UpdatedAt, DateTime.UtcNow)
            .With(customer => customer.IsDeleted, false)
            .Create();

        var other = _fixture.Build<Customer>()
            .With(other => other.Phone, "11977776666")
            .With(other => other.Document, "98765432100")
            .With(other => other.CreatedAt, DateTime.UtcNow)
            .With(other => other.UpdatedAt, DateTime.UtcNow)
            .With(other => other.IsDeleted, false)
            .Create();

        await factory.Context.Customers.AddRangeAsync([customer, other], TestContext.Current.CancellationToken);
        await factory.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        /* arrange: create a request trying to take the document already owned by the other customer */
        var request = _fixture.Build<UpdateCustomerScheme>()
            .With(scheme => scheme.Name, "João da Silva")
            .With(scheme => scheme.Phone, "(11) 96666-5555")
            .With(scheme => scheme.Document, "987.654.321-00")
            .Create();

        /* act: send PUT request to the customers endpoint */
        var response = await httpClient.PutAsJsonAsync($"/api/v1/customers/{customer.Id}", request, TestContext.Current.CancellationToken);
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

    [Fact(DisplayName = "[e2e] - when DELETE /api/v1/customers/{id} is called with an existing customer, 204 is returned")]
    public async Task When_DeleteCustomer_IsCalled_WithExistingCustomer_Then_204NoContentIsReturned()
    {
        /* arrange: persist a customer to be deleted */
        var httpClient = factory.HttpClient;
        var customer = _fixture.Build<Customer>()
            .With(customer => customer.Phone, "11988887777")
            .With(customer => customer.Document, "12345678909")
            .With(customer => customer.CreatedAt, DateTime.UtcNow)
            .With(customer => customer.UpdatedAt, DateTime.UtcNow)
            .With(customer => customer.IsDeleted, false)
            .Create();

        await factory.Context.Customers.AddAsync(customer, TestContext.Current.CancellationToken);
        await factory.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        /* act: send DELETE request to the customers endpoint */
        var response = await httpClient.DeleteAsync($"/api/v1/customers/{customer.Id}", TestContext.Current.CancellationToken);

        /* assert: verify http response status */
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        /* assert: verify the customer was soft deleted */
        var repository = factory.Services.GetRequiredService<ICustomerRepository>();
        var filters = CustomerFilters.AsBuilder()
            .WithIdentifier(customer.Id)
            .Build();

        var customersAfter = await repository.FilterCustomersAsync(filters, TestContext.Current.CancellationToken);
        var deletedCustomer = customersAfter.FirstOrDefault();

        Assert.NotNull(deletedCustomer);
        Assert.True(deletedCustomer.IsDeleted);
    }

    [Fact(DisplayName = "[e2e] - when DELETE /api/v1/customers/{id} is called with a non-existent customer, 404 is returned")]
    public async Task When_DeleteCustomer_IsCalled_WithNonExistentCustomer_Then_404NotFoundIsReturned()
    {
        /* arrange: resolves http client instance from integration environment */
        var httpClient = factory.HttpClient;

        /* act: send DELETE request targeting a customer that does not exist */
        var response = await httpClient.DeleteAsync("/api/v1/customers/customer_d3TjZUiCvu22Uf2LkapbNWjKa", TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        /* assert: verify http response status and content */
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(content));

        /* assert: deserialize response */
        var result = JsonSerializer.Deserialize<Error>(content, _serializer);

        /* assert: validate returned error */
        Assert.NotNull(result);
        Assert.Equal(CustomerErrors.DoesNotExist, result);
    }

    [Fact(DisplayName = "[e2e] - when POST /api/v1/customers/import is called with a valid csv, 202 is returned with no content")]
    public async Task When_ImportCustomers_IsCalled_WithValidCsv_Then_202AcceptedIsReturnedWithNoContent()
    {
        /* arrange: resolve the http client and build a valid csv upload */
        var httpClient = factory.HttpClient;
        var content = "document;name;phone\n52998224725;João da Silva;11988887777\n11144477735;Maria Oliveira;21977776666";

        using var form = new MultipartFormDataContent();

        form.Add(new ByteArrayContent(Encoding.UTF8.GetBytes(content)), "file", "customers.csv");

        /* act: send the multipart POST request to the import endpoint */
        var response = await httpClient.PostAsync("/api/v1/customers/import", form, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        /* assert: verify the import was accepted with no content */
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        Assert.True(string.IsNullOrEmpty(body));
    }

    [Fact(DisplayName = "[e2e] - when POST /api/v1/customers/import is called with a non-csv file, 400 is returned")]
    public async Task When_ImportCustomers_IsCalled_WithNonCsvFile_Then_400BadRequestIsReturned()
    {
        /* arrange: resolve the http client and build an upload that is not a csv */
        var httpClient = factory.HttpClient;

        using var form = new MultipartFormDataContent();

        form.Add(new ByteArrayContent(Encoding.UTF8.GetBytes("not a csv file")), "file", "customers.txt");

        /* act: send the multipart POST request to the import endpoint */
        var response = await httpClient.PostAsync("/api/v1/customers/import", form, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        /* assert: verify the request was rejected by validation */
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(body));
    }

    [Fact(DisplayName = "[e2e] - when POST /api/v1/customers/import is called without a file, 400 is returned")]
    public async Task When_ImportCustomers_IsCalled_WithoutFile_Then_400BadRequestIsReturned()
    {
        /* arrange: resolve the http client and build a multipart request carrying no file */
        var httpClient = factory.HttpClient;
        using var form = new MultipartFormDataContent
        {
            { new StringContent("placeholder"), "unused" }
        };

        /* act: send the multipart POST request to the import endpoint */
        var response = await httpClient.PostAsync("/api/v1/customers/import", form, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        /* assert: verify the request was rejected by validation */
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(body));
    }

    public async ValueTask DisposeAsync() => await ValueTask.FromResult(ValueTask.CompletedTask);
    public async ValueTask InitializeAsync() => await factory.CleanDatabaseAsync();
}
