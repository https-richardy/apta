namespace Apta.WebUI.Clients;

public sealed class AuthenticationClient(HttpClient httpClient, ISettings settings) : IAuthenticationClient
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly JsonSerializerOptions _serializer = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true
    };

    public async Task<Result<AuthenticationScheme>> AuthenticateAsync(AuthenticationCredentialsScheme credentials, CancellationToken cancellation = default)
    {
        // https://www.rfc-editor.org/rfc/rfc6749#section-4.3.2
        // use resource owner password credentials grant to exchange the user login for tokens

        var content = new FormUrlEncodedContent(
        [
            new("grant_type", "password"),
            new("scope", "openid"),

            new("client_id", settings.Authentication.Audience),
            new("username", credentials.Email),
            new("password", credentials.Password),

        ]);

        var response = await _httpClient.PostAsync("protocol/openid-connect/token", content, cancellation);
        var contentString = await response.Content.ReadAsStringAsync(cancellation);

        if (!response.IsSuccessStatusCode)
        {
            return Result<AuthenticationScheme>.Failure(Error.Unknown);
        }

        var authentication = JsonSerializer.Deserialize<AuthenticationScheme>(contentString, _serializer);

        return authentication is not null
            ? Result<AuthenticationScheme>.Success(authentication)
            : Result<AuthenticationScheme>.Failure(Error.Unknown);
    }

    public async Task<Result> LogoutAsync(String refreshToken, CancellationToken cancellation = default)
    {
        // https://www.rfc-editor.org/rfc/rfc7009
        // revoke the keycloak session bound to the user's refresh token (backchannel logout)

        var content = new FormUrlEncodedContent(
        [
            new("client_id", settings.Authentication.Audience),
            new("refresh_token", refreshToken)
        ]);

        var response = await _httpClient.PostAsync("protocol/openid-connect/logout", content, cancellation);

        return response.IsSuccessStatusCode
            ? Result.Success()
            : Result.Failure(Error.Unknown);
    }
}
