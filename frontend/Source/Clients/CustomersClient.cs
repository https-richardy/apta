namespace Apta.WebUI.Clients;

public sealed class CustomersClient(HttpClient httpClient) : ICustomersClient
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly JsonSerializerOptions _serializer = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public async Task<Result<Pagination<CustomerScheme>>> GetCustomersAsync(CustomerFiltersParameters parameters, CancellationToken cancellation = default)
    {
        var queryString = QueryParametersParser.ToQueryString(parameters);

        var response = await _httpClient.GetAsync($"/api/v1/customers?{queryString}", cancellation);
        var content = await response.Content.ReadAsStringAsync(cancellation);

        if (!response.IsSuccessStatusCode)
        {
            return Result<Pagination<CustomerScheme>>.Failure(Error.Unknown);
        }

        var customers = JsonSerializer.Deserialize<Pagination<CustomerScheme>>(content, _serializer);

        return customers is null
            ? Result<Pagination<CustomerScheme>>.Failure(Error.Unknown)
            : Result<Pagination<CustomerScheme>>.Success(customers);
    }
}
