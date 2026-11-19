namespace Apta.WebUI.Clients;

public sealed class CustomersClient(HttpClient httpClient, INotificationService notificationService) : ICustomersClient
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly INotificationService _notificationService = notificationService;
    private readonly JsonSerializerOptions _serializer = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public async Task<Result<CustomerScheme>> CreateCustomerAsync(CustomerCreationScheme parameters, CancellationToken cancellation = default)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/v1/customers", parameters, _serializer, cancellation);
        var content = await response.Content.ReadAsStringAsync(cancellation);

        if (!response.IsSuccessStatusCode)
        {
            var error = JsonSerializer.Deserialize<Error>(content, _serializer)!;

            await _notificationService.CloseAllMessageBarsAsync();
            await _notificationService.ShowErrorToastAsync(error.Code, error.Description);

            return error is not null
                ? Result<CustomerScheme>.Failure(error)
                : Result<CustomerScheme>.Failure(Error.Unknown);
        }

        var customer = JsonSerializer.Deserialize<CustomerScheme>(content, _serializer);

        return customer is not null
            ? Result<CustomerScheme>.Success(customer)
            : Result<CustomerScheme>.Failure(Error.Unknown);
    }

    public async Task<Result<CustomerScheme>> UpdateCustomerAsync(UpdateCustomerScheme parameters, CancellationToken cancellation = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/v1/customers/{parameters.Id}", parameters, _serializer, cancellation);
        var content = await response.Content.ReadAsStringAsync(cancellation);

        if (!response.IsSuccessStatusCode)
        {
            var error = JsonSerializer.Deserialize<Error>(content, _serializer)!;

            await _notificationService.CloseAllMessageBarsAsync();
            await _notificationService.ShowErrorToastAsync(error.Code, error.Description);

            return error is not null
                ? Result<CustomerScheme>.Failure(error)
                : Result<CustomerScheme>.Failure(Error.Unknown);
        }

        var customer = JsonSerializer.Deserialize<CustomerScheme>(content, _serializer);

        return customer is not null
            ? Result<CustomerScheme>.Success(customer)
            : Result<CustomerScheme>.Failure(Error.Unknown);
    }

    public async Task<Result> DeleteCustomerAsync(CustomerDeletionScheme parameters, CancellationToken cancellation = default)
    {
        var response = await _httpClient.DeleteAsync($"/api/v1/customers/{parameters.Id}", cancellation);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellation);
            var error = JsonSerializer.Deserialize<Error>(content, _serializer)!;

            await _notificationService.CloseAllMessageBarsAsync();
            await _notificationService.ShowErrorToastAsync(error.Code, error.Description);

            return error is not null
                ? Result.Failure(error)
                : Result.Failure(Error.Unknown);
        }

        return Result.Success();
    }

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

    public async Task<Result> ImportCustomersAsync(CustomersImportScheme parameters, CancellationToken cancellation = default)
    {
        using var payload = new MultipartFormDataContent();
        using var file = new StreamContent(parameters.Content);

        file.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Text.Csv);
        payload.Add(file, "file", parameters.FileName);

        var response = await _httpClient.PostAsync("/api/v1/customers/import", payload, cancellation);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellation);
            var error = JsonSerializer.Deserialize<Error>(content, _serializer)!;

            await _notificationService.CloseAllMessageBarsAsync();
            await _notificationService.ShowErrorToastAsync(error.Code, error.Description);

            return error is not null
                ? Result.Failure(error)
                : Result.Failure(Error.Unknown);
        }

        return Result.Success();
    }
}
