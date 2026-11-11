namespace Apta.WebUI.Clients;

public sealed class AdministratorsClient(HttpClient httpClient, INotificationService notificationService) : IAdministratorsClient
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly INotificationService _notificationService = notificationService;
    private readonly JsonSerializerOptions _serializer = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public async Task<Result<AdministratorScheme>> CreateAdministratorAsync(AdministratorCreationScheme parameters, CancellationToken cancellation = default)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/v1/administrators", parameters, _serializer, cancellation);
        var content = await response.Content.ReadAsStringAsync(cancellation);

        if (!response.IsSuccessStatusCode)
        {
            var error = JsonSerializer.Deserialize<Error>(content, _serializer)!;

            await _notificationService.CloseAllMessageBarsAsync();
            await _notificationService.ShowErrorToastAsync(error.Code, error.Description);

            return error is not null
                ? Result<AdministratorScheme>.Failure(error)
                : Result<AdministratorScheme>.Failure(Error.Unknown);
        }

        var administrator = JsonSerializer.Deserialize<AdministratorScheme>(content, _serializer);

        return administrator is not null
            ? Result<AdministratorScheme>.Success(administrator)
            : Result<AdministratorScheme>.Failure(Error.Unknown);
    }

    public async Task<Result<AdministratorScheme>> UpdateAdministratorAsync(UpdateAdministratorScheme parameters, CancellationToken cancellation = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/v1/administrators/{parameters.Id}", parameters, _serializer, cancellation);
        var content = await response.Content.ReadAsStringAsync(cancellation);

        if (!response.IsSuccessStatusCode)
        {
            var error = JsonSerializer.Deserialize<Error>(content, _serializer)!;

            await _notificationService.CloseAllMessageBarsAsync();
            await _notificationService.ShowErrorToastAsync(error.Code, error.Description);

            return error is not null
                ? Result<AdministratorScheme>.Failure(error)
                : Result<AdministratorScheme>.Failure(Error.Unknown);
        }

        var administrator = JsonSerializer.Deserialize<AdministratorScheme>(content, _serializer);

        return administrator is not null
            ? Result<AdministratorScheme>.Success(administrator)
            : Result<AdministratorScheme>.Failure(Error.Unknown);
    }

    public async Task<Result> DeleteAdministratorAsync(AdministratorDeletionScheme parameters, CancellationToken cancellation = default)
    {
        var response = await _httpClient.DeleteAsync($"/api/v1/administrators/{parameters.Id}", cancellation);

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

    public async Task<Result<Pagination<AdministratorScheme>>> GetAdministratorsAsync(AdministratorFiltersParameters parameters, CancellationToken cancellation = default)
    {
        var queryString = QueryParametersParser.ToQueryString(parameters);

        var response = await _httpClient.GetAsync($"/api/v1/administrators?{queryString}", cancellation);
        var content = await response.Content.ReadAsStringAsync(cancellation);

        if (!response.IsSuccessStatusCode)
        {
            return Result<Pagination<AdministratorScheme>>.Failure(Error.Unknown);
        }

        var administrators = JsonSerializer.Deserialize<Pagination<AdministratorScheme>>(content, _serializer);

        return administrators is null
            ? Result<Pagination<AdministratorScheme>>.Failure(Error.Unknown)
            : Result<Pagination<AdministratorScheme>>.Success(administrators);
    }
}
