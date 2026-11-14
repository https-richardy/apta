namespace Apta.WebUI.Tests.Unit.Components.Customers;

public sealed class CustomerCreationFormTests : BunitContext
{
    private readonly Fixture _fixture = new();

    public CustomerCreationFormTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddFluentUIComponents();
    }

    [Fact(DisplayName = "[component] - when rendered, the name, phone and document fields are shown with the save and cancel actions")]
    public void When_Rendered_Then_FieldsAndActionsAreShown()
    {
        /* act: render the creation form with its default model */
        var form = Render<CustomerCreationForm>();

        /* assert: every field and both actions are present */
        Assert.Contains("Nome", form.Markup);
        Assert.Contains("Telefone", form.Markup);
        Assert.Contains("CPF", form.Markup);
        Assert.Contains("Salvar", form.Markup);
        Assert.Contains("Cancelar", form.Markup);
    }

    [Fact(DisplayName = "[component] - when the model carries values, the inputs are pre-populated with them")]
    public void When_ModelCarriesValues_Then_InputsArePrePopulated()
    {
        /* arrange: a model carrying a valid name, phone and document */
        var model = _fixture.Build<CustomerCreationScheme>()
            .With(customer => customer.Name, "João da Silva")
            .With(customer => customer.Phone, "(11) 98888-7777")
            .With(customer => customer.Document, "123.456.789-09")
            .Create();

        /* act: render the form bound to the model */
        var form = Render<CustomerCreationForm>(parameters => parameters
            .Add(component => component.Model, model));

        /* assert: every value reaches the rendered inputs */
        Assert.Contains("João da Silva", form.Markup);
        Assert.Contains("(11) 98888-7777", form.Markup);
        Assert.Contains("123.456.789-09", form.Markup);
    }

    [Fact(DisplayName = "[component] - when submitted with only the name, OnValidSubmit is invoked because a lead may have just the name")]
    public void When_SubmittedWithOnlyName_Then_OnValidSubmitIsInvoked()
    {
        /* arrange: a model with just the name, leaving phone and document empty */
        var model = _fixture.Build<CustomerCreationScheme>()
            .With(customer => customer.Name, "João da Silva")
            .Without(customer => customer.Phone)
            .Without(customer => customer.Document)
            .Create();

        var submitted = false;
        var form = Render<CustomerCreationForm>(parameters => parameters
            .Add(component => component.Model, model)
            .Add(component => component.OnValidSubmit, EventCallback.Factory.Create(this, () => submitted = true)));

        /* act: submit the form */
        form.Find("form").Submit();

        /* assert: the valid submit callback was invoked */
        Assert.True(submitted);
    }

    [Fact(DisplayName = "[component] - when submitted without a name, OnValidSubmit is blocked and the required message is shown")]
    public void When_SubmittedWithoutName_Then_OnValidSubmitIsBlockedAndRequiredMessageIsShown()
    {
        /* arrange: a model without a name and without the optional fields */
        var model = _fixture.Build<CustomerCreationScheme>()
            .With(customer => customer.Name, string.Empty)
            .Without(customer => customer.Phone)
            .Without(customer => customer.Document)
            .Create();

        var submitted = false;
        var form = Render<CustomerCreationForm>(parameters => parameters
            .Add(component => component.Model, model)
            .Add(component => component.OnValidSubmit, EventCallback.Factory.Create(this, () => submitted = true)));

        /* act: submit the invalid form */
        form.Find("form").Submit();

        /* assert: the submit was blocked and the required name message surfaced */
        Assert.False(submitted);
        Assert.Contains("Informe o nome do cliente.", form.Markup);
    }

    [Fact(DisplayName = "[component] - when submitted with an invalid phone, OnValidSubmit is blocked and the phone format message is shown")]
    public void When_SubmittedWithInvalidPhone_Then_OnValidSubmitIsBlockedAndPhoneMessageIsShown()
    {
        /* arrange: a model with a valid name but a malformed phone */
        var model = _fixture.Build<CustomerCreationScheme>()
            .With(customer => customer.Name, "João da Silva")
            .With(customer => customer.Phone, "123")
            .Without(customer => customer.Document)
            .Create();

        var submitted = false;
        var form = Render<CustomerCreationForm>(parameters => parameters
            .Add(component => component.Model, model)
            .Add(component => component.OnValidSubmit, EventCallback.Factory.Create(this, () => submitted = true)));

        /* act: submit the invalid form */
        form.Find("form").Submit();

        /* assert: the submit was blocked and the phone format message surfaced */
        Assert.False(submitted);
        Assert.Contains("Informe um telefone válido.", form.Markup);
    }

    [Fact(DisplayName = "[component] - when submitted with an invalid document, OnValidSubmit is blocked and the document format message is shown")]
    public void When_SubmittedWithInvalidDocument_Then_OnValidSubmitIsBlockedAndDocumentMessageIsShown()
    {
        /* arrange: a model with a valid name but a malformed document */
        var model = _fixture.Build<CustomerCreationScheme>()
            .With(customer => customer.Name, "João da Silva")
            .Without(customer => customer.Phone)
            .With(customer => customer.Document, "ABC")
            .Create();

        var submitted = false;
        var form = Render<CustomerCreationForm>(parameters => parameters
            .Add(component => component.Model, model)
            .Add(component => component.OnValidSubmit, EventCallback.Factory.Create(this, () => submitted = true)));

        /* act: submit the invalid form */
        form.Find("form").Submit();

        /* assert: the submit was blocked and the document format message surfaced */
        Assert.False(submitted);
        Assert.Contains("Informe um CPF válido.", form.Markup);
    }

    [Fact(DisplayName = "[component] - when the cancel action is clicked, OnCancel is invoked")]
    public void When_CancelActionIsClicked_Then_OnCancelIsInvoked()
    {
        /* arrange: a form with a captured cancel */
        var cancelled = false;
        var form = Render<CustomerCreationForm>(parameters => parameters
            .Add(component => component.OnCancel, EventCallback.Factory.Create(this, () => cancelled = true)));

        /* act: click the cancel button */
        form.FindAll("fluent-button")[1].Click();

        /* assert: the cancel callback was invoked */
        Assert.True(cancelled);
    }
}
