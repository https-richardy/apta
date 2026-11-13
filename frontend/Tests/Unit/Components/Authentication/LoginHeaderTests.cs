namespace Apta.WebUI.Tests.Unit.Components.Authentication;

public sealed class LoginHeaderTests : BunitContext
{
    public LoginHeaderTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddFluentUIComponents();
    }

    [Fact(DisplayName = "[component] - when rendered, the login header shows the sign-in title and the contact hint")]
    public void When_Rendered_Then_SignInTitleAndContactHintAreShown()
    {
        /* act: render the login header */
        var header = Render<LoginHeader>();

        /* assert: the sign-in title and the contact hint are shown */
        Assert.Contains("Faça login na sua conta.", header.Markup);
        Assert.Contains("Contate o administrador", header.Markup);
    }
}
