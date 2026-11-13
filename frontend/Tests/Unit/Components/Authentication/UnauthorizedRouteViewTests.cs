namespace Apta.WebUI.Tests.Unit.Components.Authentication;

public sealed class UnauthorizedRouteViewTests : BunitContext
{
    public UnauthorizedRouteViewTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddFluentUIComponents();
    }

    [Fact(DisplayName = "[component] - when the user is authenticated, the forbidden view is rendered without navigating")]
    public void When_UserIsAuthenticated_Then_ForbiddenViewIsRenderedWithoutNavigating()
    {
        /* arrange: an authenticated authentication state */
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.Name, "operator")], authenticationType: "test");
        var authentication = new AuthenticationState(new ClaimsPrincipal(identity));

        var navigation = Services.GetRequiredService<NavigationManager>();
        var origin = navigation.Uri;

        /* act: render the view with the authenticated state */
        var view = Render<UnauthorizedRouteView>(parameters => parameters
            .Add(component => component.Authentication, authentication));

        /* assert: the forbidden view is shown */
        Assert.NotEmpty(view.FindComponents<Forbidden>());
        Assert.Equal(origin, navigation.Uri);
    }

    [Fact(DisplayName = "[component] - when the user is not authenticated, it redirects to the authentication route")]
    public void When_UserIsNotAuthenticated_Then_ItRedirectsToTheAuthenticationRoute()
    {
        /* arrange: an anonymous authentication state */
        var authentication = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        /* act: render the view with the anonymous state */
        var view = Render<UnauthorizedRouteView>(parameters => parameters
            .Add(component => component.Authentication, authentication));

        /* assert: the forbidden view is not shown */
        Assert.Empty(view.FindComponents<Forbidden>());

        /* assert: the user was redirected to the authentication route */
        var navigation = Services.GetRequiredService<NavigationManager>();

        Assert.EndsWith("/authentication/protocol/connect/", navigation.Uri);
    }
}
