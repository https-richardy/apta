namespace Apta.WebUI.Authentication;

public sealed class JwtBearerAuthenticationStateProvider(ILocalStorageService localStorage) : AuthenticationStateProvider
{
    private readonly JwtSecurityTokenHandler _handler = new();

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await localStorage.GetItemAsStringAsync("apta:authentication");
        var anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        if (string.IsNullOrWhiteSpace(token) || !_handler.CanReadToken(token))
            return anonymous;

        var jwt = _handler.ReadJwtToken(token);
        if (jwt.ValidTo != DateTime.MinValue && jwt.ValidTo <= DateTime.UtcNow)
            return anonymous;

        var identity = new ClaimsIdentity(claims: jwt.Claims, authenticationType: "jwt", nameType: "name", roleType: "role");

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task AuthenticateAsync(string accessToken, string refreshToken)
    {
        await localStorage.SetItemAsStringAsync("apta:authentication", accessToken);
        await localStorage.SetItemAsStringAsync("apta:refresh", refreshToken);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task<string?> GetRefreshTokenAsync() =>
        await localStorage.GetItemAsStringAsync("apta:refresh");

    public async Task LogoutAsync()
    {
        await localStorage.RemoveItemsAsync(["apta:authentication", "apta:refresh"]);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
    }
}
