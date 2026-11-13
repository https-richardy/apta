namespace Apta.WebUI.Tests.Unit.Components.Authentication;

public sealed class LoginPageTests : BunitContext
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IAuthenticationClient> _client = new();
    private readonly Mock<ILocalStorageService> _localStorage = new();

    public LoginPageTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;

        Services.AddFluentUIComponents();
        Services.AddDotNetQuery(options =>
        {
            options.ExecutionMode = QueryExecutionMode.Csr;
            options.StaleTime = TimeSpan.Zero;
            options.CacheTime = TimeSpan.FromMinutes(5);
        });

        Services.AddSingleton(_client.Object);
        Services.AddSingleton(_localStorage.Object);

        Services.AddScoped<JwtBearerAuthenticationStateProvider>();
        Services.AddScoped<AuthenticationService>();
    }

    [Fact(DisplayName = "[component] - when rendered, the login header and form are shown")]
    public void When_Rendered_Then_LoginHeaderAndFormAreShown()
    {
        /* act: render the page with an anonymous authentication state */
        var anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        var component = Render<Pages.Authentication.Login>(parameters => parameters
            .AddCascadingValue(Task.FromResult(anonymous)));

        /* assert: the header and the login form are shown */
        Assert.NotEmpty(component.FindComponents<LoginHeader>());
        Assert.NotEmpty(component.FindComponents<LoginForm>());
    }

    [Fact(DisplayName = "[component] - when the user is already authenticated, the page redirects to the home route")]
    public void When_AlreadyAuthenticated_Then_PageRedirectsToHome()
    {
        /* arrange: an authenticated state carried by the cascading parameter */
        var principal = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, "john.doe@apta.com.br"),
            new Claim(ClaimTypes.NameIdentifier, "user_administrator_d3TjZUiCvu32Uq2LiapbNWjKa")
        ]);

        var authenticated = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(principal, authenticationType: "jwt")));
        var component = Render<Pages.Authentication.Login>(parameters => parameters
            .AddCascadingValue(Task.FromResult(authenticated)));

        /* assert: the page navigated to the home route */
        var navigation = Services.GetRequiredService<BunitNavigationManager>();

        component.WaitForAssertion(() => Assert.Contains(navigation.History, entry => entry.Uri == "/"));
    }

    [Fact(DisplayName = "[component] - when the user is not authenticated, the page stays on the login form")]
    public void When_NotAuthenticated_Then_PageStaysOnTheLoginForm()
    {
        /* arrange: an anonymous state carried by the cascading parameter */
        var anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        /* act: render the page */
        var component = Render<Pages.Authentication.Login>(parameters => parameters
            .AddCascadingValue(Task.FromResult(anonymous)));

        /* assert: no navigation happened and the login form is still shown */
        var navigation = Services.GetRequiredService<BunitNavigationManager>();

        Assert.Empty(navigation.History);
        Assert.NotEmpty(component.FindComponents<LoginForm>());
    }

    [Fact(DisplayName = "[component] - when valid credentials are submitted, the user is authenticated through the client")]
    public void When_ValidCredentialsSubmitted_Then_UserIsAuthenticatedThroughTheClient()
    {
        /* arrange: an anonymous state and a client that authenticates successfully */
        var anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        var authentication = _fixture.Create<AuthenticationScheme>();

        _client.Setup(client => client.AuthenticateAsync(It.IsAny<AuthenticationCredentialsScheme>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AuthenticationScheme>.Success(authentication));

        var component = Render<Pages.Authentication.Login>(parameters => parameters
            .AddCascadingValue(Task.FromResult(anonymous)));

        /* act: fill the credentials and submit the form */
        component.FindAll("fluent-text-input")[0].Change("john.doe@apta.com.br");
        component.FindAll("fluent-text-input")[1].Change("1234567890");
        component.Find("form").Submit();

        /* assert: the credentials were authenticated through the client */
        component.WaitForAssertion(() => _client.Verify(client => client.AuthenticateAsync(
                It.Is<AuthenticationCredentialsScheme>(credentials => credentials.Email == "john.doe@apta.com.br" && credentials.Password == "1234567890"),
                It.IsAny<CancellationToken>()), Times.Once)
        );
    }
}
