namespace Apta.WebUI.Tests.Unit.Components.Customers;

public sealed class CustomerUpdateFormTests : BunitContext
{
    private readonly Fixture _fixture = new();

    public CustomerUpdateFormTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddFluentUIComponents();
    }

    [Fact(DisplayName = "[component] - when rendered, the name, phone and document fields are shown with the save changes and cancel actions")]
    public void When_Rendered_Then_FieldsAndActionsAreShown()
    {
        /* act: render the update form with its default model */
        var form = Render<CustomerUpdateForm>();

        /* assert: every field and both actions are present */
        Assert.Contains("Nome", form.Markup);
        Assert.Contains("Telefone", form.Markup);
        Assert.Contains("CPF", form.Markup);
        Assert.Contains("Salvar alterações", form.Markup);
        Assert.Contains("Cancelar", form.Markup);
    }

    [Fact(DisplayName = "[component] - when the model carries the customer being edited, the inputs are pre-populated with its values")]
    public void When_ModelCarriesTheEditedCustomer_Then_InputsArePrePopulated()
    {
        /* arrange: a model carrying the customer currently being edited */
        var model = _fixture.Build<UpdateCustomerScheme>()
            .With(customer => customer.Name, "João da Silva")
            .With(customer => customer.Phone, "(11) 98888-7777")
            .With(customer => customer.Document, "123.456.789-09")
            .Create();

        /* act: render the form bound to the model */
        var form = Render<CustomerUpdateForm>(parameters => parameters
            .Add(component => component.Model, model));

        /* assert: every value reaches the rendered inputs */
        Assert.Contains("João da Silva", form.Markup);
        Assert.Contains("(11) 98888-7777", form.Markup);
        Assert.Contains("123.456.789-09", form.Markup);
    }

    [Theory(DisplayName = "[component] - when submitted with only the name, OnValidSubmit is invoked because phone and document are optional, whether they are null or empty")]
    [InlineData(null, null)]
    [InlineData("", "")]
    public void When_SubmittedWithOnlyName_Then_OnValidSubmitIsInvoked(string? phone, string? document)
    {
        /* arrange: a model with just the name, leaving phone and document null or empty */
        var model = _fixture.Build<UpdateCustomerScheme>()
            .With(customer => customer.Name, "João da Silva")
            .With(customer => customer.Phone, phone)
            .With(customer => customer.Document, document)
            .Create();

        var submitted = false;
        var form = Render<CustomerUpdateForm>(parameters => parameters
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
        var model = _fixture.Build<UpdateCustomerScheme>()
            .With(customer => customer.Name, string.Empty)
            .Without(customer => customer.Phone)
            .Without(customer => customer.Document)
            .Create();

        var submitted = false;
        var form = Render<CustomerUpdateForm>(parameters => parameters
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
        var model = _fixture.Build<UpdateCustomerScheme>()
            .With(customer => customer.Name, "João da Silva")
            .With(customer => customer.Phone, "123")
            .Without(customer => customer.Document)
            .Create();

        var submitted = false;
        var form = Render<CustomerUpdateForm>(parameters => parameters
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
        var model = _fixture.Build<UpdateCustomerScheme>()
            .With(customer => customer.Name, "João da Silva")
            .Without(customer => customer.Phone)
            .With(customer => customer.Document, "ABC")
            .Create();

        var submitted = false;
        var form = Render<CustomerUpdateForm>(parameters => parameters
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
        var form = Render<CustomerUpdateForm>(parameters => parameters
            .Add(component => component.OnCancel, EventCallback.Factory.Create(this, () => cancelled = true)));

        /* act: click the cancel button */
        form.FindAll("fluent-button")[1].Click();

        /* assert: the cancel callback was invoked */
        Assert.True(cancelled);
    }
}
