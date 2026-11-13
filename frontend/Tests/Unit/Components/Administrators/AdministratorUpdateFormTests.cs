namespace Apta.WebUI.Tests.Unit.Components.Administrators;

public sealed class AdministratorUpdateFormTests : BunitContext
{
    private readonly Fixture _fixture = new();

    public AdministratorUpdateFormTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddFluentUIComponents();
    }

    [Fact(DisplayName = "[component] - when rendered, the name and code fields are shown with the save changes and cancel actions")]
    public void When_Rendered_Then_FieldsAndActionsAreShown()
    {
        /* act: render the update form with its default model */
        var form = Render<AdministratorUpdateForm>();

        /* assert: both fields and both actions are present */
        Assert.Contains("Nome", form.Markup);
        Assert.Contains("Código BACEN", form.Markup);
        Assert.Contains("Salvar alterações", form.Markup);
        Assert.Contains("Cancelar", form.Markup);
    }

    [Fact(DisplayName = "[component] - when the model carries the administrator being edited, the inputs are pre-populated with its values")]
    public void When_ModelCarriesTheEditedAdministrator_Then_InputsArePrePopulated()
    {
        /* arrange: a model carrying the administrator currently being edited */
        var model = _fixture.Build<UpdateAdministratorScheme>()
            .With(administrator => administrator.Name, "EMBRACON")
            .With(administrator => administrator.Code, "58113812")
            .Create();

        /* act: render the form bound to the model */
        var form = Render<AdministratorUpdateForm>(parameters => parameters
            .Add(component => component.Model, model));

        /* assert: both values reach the rendered inputs */
        Assert.Contains("EMBRACON", form.Markup);
        Assert.Contains("58113812", form.Markup);
    }

    [Fact(DisplayName = "[component] - when submitted with a valid model, OnValidSubmit is invoked")]
    public void When_SubmittedWithValidModel_Then_OnValidSubmitIsInvoked()
    {
        /* arrange: a valid model and a captured submit */
        var model = _fixture.Build<UpdateAdministratorScheme>()
            .With(administrator => administrator.Name, "EMBRACON")
            .With(administrator => administrator.Code, "58113812")
            .Create();

        var submitted = false;
        var form = Render<AdministratorUpdateForm>(parameters => parameters
            .Add(component => component.Model, model)
            .Add(component => component.OnValidSubmit, EventCallback.Factory.Create(this, () => submitted = true)));

        /* act: submit the form */
        form.Find("form").Submit();

        /* assert: the valid submit callback was invoked */
        Assert.True(submitted);
    }

    [Fact(DisplayName = "[component] - when submitted with an empty model, OnValidSubmit is blocked and the required messages are shown")]
    public void When_SubmittedWithEmptyModel_Then_OnValidSubmitIsBlockedAndRequiredMessagesAreShown()
    {
        /* arrange: an empty model and a captured submit */
        var model = _fixture.Build<UpdateAdministratorScheme>()
            .With(administrator => administrator.Name, string.Empty)
            .With(administrator => administrator.Code, string.Empty)
            .Create();

        var submitted = false;
        var form = Render<AdministratorUpdateForm>(parameters => parameters
            .Add(component => component.Model, model)
            .Add(component => component.OnValidSubmit, EventCallback.Factory.Create(this, () => submitted = true)));

        /* act: submit the invalid form */
        form.Find("form").Submit();

        /* assert: the submit was blocked and both required messages surfaced */
        Assert.False(submitted);
        Assert.Contains("Informe o nome da administradora.", form.Markup);
        Assert.Contains("Informe o código BACEN.", form.Markup);
    }

    [Fact(DisplayName = "[component] - when submitted with a non-numeric code, OnValidSubmit is blocked and the code format message is shown")]
    public void When_SubmittedWithMalformedCode_Then_OnValidSubmitIsBlockedAndCodeFormatMessageIsShown()
    {
        /* arrange: a model with a valid name but a code that is not 8 digits */
        var model = _fixture.Build<UpdateAdministratorScheme>()
            .With(administrator => administrator.Name, "EMBRACON")
            .With(administrator => administrator.Code, "ABC123")
            .Create();

        var submitted = false;
        var form = Render<AdministratorUpdateForm>(parameters => parameters
            .Add(component => component.Model, model)
            .Add(component => component.OnValidSubmit, EventCallback.Factory.Create(this, () => submitted = true)));

        /* act: submit the invalid form */
        form.Find("form").Submit();

        /* assert: the submit was blocked and the code format message surfaced */
        Assert.False(submitted);
        Assert.Contains("O código BACEN deve conter 8 dígitos.", form.Markup);
    }

    [Fact(DisplayName = "[component] - when the cancel action is clicked, OnCancel is invoked")]
    public void When_CancelActionIsClicked_Then_OnCancelIsInvoked()
    {
        /* arrange: a form with a captured cancel */
        var cancelled = false;
        var form = Render<AdministratorUpdateForm>(parameters => parameters
            .Add(component => component.OnCancel, EventCallback.Factory.Create(this, () => cancelled = true)));

        /* act: click the cancel button */
        form.FindAll("fluent-button")[1].Click();

        /* assert: the cancel callback was invoked */
        Assert.True(cancelled);
    }
}
