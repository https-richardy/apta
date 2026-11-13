namespace Apta.WebUI.Tests.Unit.Components.Authentication;

public sealed class LoginFormTests : BunitContext
{
    public LoginFormTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddFluentUIComponents();
    }

    [Fact(DisplayName = "[component] - when rendered, the form shows the email and password fields")]
    public void When_Rendered_Then_EmailAndPasswordFieldsAreShown()
    {
        /* act: render the login form */
        var form = Render<LoginForm>();
        var fields = form.FindComponents<LoginField>();

        Assert.Equal(2, fields.Count);

        /* assert: the fields are labelled for email and password */
        Assert.Contains("Email", form.Markup);
        Assert.Contains("Senha", form.Markup);
    }

    [Fact(DisplayName = "[component] - when rendered, the submit button and the terms notice are shown")]
    public void When_Rendered_Then_SubmitButtonAndTermsNoticeAreShown()
    {
        /* act: render the login form */
        var form = Render<LoginForm>();

        /* assert: the primary submit action is shown */
        Assert.Contains("Entrar", form.Markup);

        /* assert: the terms and privacy notice is shown */
        Assert.Contains("Termos de Serviço", form.Markup);
        Assert.Contains("Política de Privacidade", form.Markup);
    }

    [Fact(DisplayName = "[component] - when submitted with valid credentials, OnValidSubmit is invoked")]
    public void When_SubmittedWithValidCredentials_Then_OnValidSubmitIsInvoked()
    {
        /* arrange: valid credentials and a captured submit */
        var credentials = new AuthenticationCredentialsScheme
        {
            Email = "john.doe@apta.com.br",
            Password = "1234567890",
        };

        var submitted = false;
        var form = Render<LoginForm>(parameters => parameters
            .Add(component => component.Credentials, credentials)
            .Add(component => component.OnValidSubmit, EventCallback.Factory.Create(this, () => submitted = true)));

        /* act: submit the form */
        form.Find("form").Submit();

        /* assert: the valid submit callback was invoked */
        Assert.True(submitted);
    }
}
