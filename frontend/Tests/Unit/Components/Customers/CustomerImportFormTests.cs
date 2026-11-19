namespace Apta.WebUI.Tests.Unit.Components.Customers;

public sealed class CustomerImportFormTests : BunitContext
{
    public CustomerImportFormTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddFluentUIComponents();
    }

    [Fact(DisplayName = "[component] - when rendered, the drop zone instructions are shown with the import and cancel actions")]
    public void When_Rendered_Then_InstructionsAndActionsAreShown()
    {
        /* act: render the import form with its default model */
        var form = Render<CustomerImportForm>();

        /* assert: the drop zone instructions are present */
        Assert.Contains("Arraste o arquivo CSV aqui ou clique para selecionar", form.Markup);
        Assert.Contains("Apenas arquivos .csv, até 5 MB.", form.Markup);

        /* assert: both actions are present */
        Assert.Contains("Importar", form.Markup);
        Assert.Contains("Cancelar", form.Markup);
    }

    [Fact(DisplayName = "[component] - when the model carries a ready file, the success bar shows its name")]
    public void When_ModelCarriesReadyFile_Then_SuccessBarShowsFileName()
    {
        /* arrange: a model carrying a ready csv content and file name */
        var model = new CustomersImportScheme
        {
            Content = new MemoryStream([1, 2, 3]),
            FileName = "clientes.csv"
        };

        /* act: render the form bound to the model */
        var form = Render<CustomerImportForm>(parameters => parameters
            .Add(component => component.Model, model));

        /* assert: the success bar surfaces with the file name */
        Assert.Contains("Arquivo pronto para importação", form.Markup);
        Assert.Contains("clientes.csv", form.Markup);
    }

    [Fact(DisplayName = "[component] - when submitted without a file, OnValidSubmit is blocked and the required message is shown")]
    public void When_SubmittedWithoutFile_Then_OnValidSubmitIsBlockedAndRequiredMessageIsShown()
    {
        /* arrange: the default model carries no content nor file name */
        var submitted = false;
        var form = Render<CustomerImportForm>(parameters => parameters
            .Add(component => component.OnValidSubmit, EventCallback.Factory.Create<CustomersImportScheme>(this, _ => submitted = true)));

        /* act: submit the empty form */
        form.Find("form").Submit();

        /* assert: the submit was blocked and the required message surfaced */
        Assert.False(submitted);
        Assert.Contains("selecione um arquivo para importar.", form.Markup);
    }

    [Fact(DisplayName = "[component] - when submitted with a non csv file, OnValidSubmit is blocked and the csv message is shown")]
    public void When_SubmittedWithNonCsvFile_Then_OnValidSubmitIsBlockedAndCsvMessageIsShown()
    {
        /* arrange: a model with content but a file that is not a csv */
        var model = new CustomersImportScheme
        {
            Content = new MemoryStream([1, 2, 3]),
            FileName = "clientes.txt"
        };

        var submitted = false;
        var form = Render<CustomerImportForm>(parameters => parameters
            .Add(component => component.Model, model)
            .Add(component => component.OnValidSubmit, EventCallback.Factory.Create<CustomersImportScheme>(this, _ => submitted = true)));

        /* act: submit the invalid form */
        form.Find("form").Submit();

        /* assert: the submit was blocked and the csv message surfaced */
        Assert.False(submitted);
        Assert.Contains("o arquivo deve ser um .csv.", form.Markup);
    }

    [Fact(DisplayName = "[component] - when submitted with a file over 5 MB, OnValidSubmit is blocked and the size message is shown")]
    public void When_SubmittedWithOversizedFile_Then_OnValidSubmitIsBlockedAndSizeMessageIsShown()
    {
        /* arrange: a csv model whose content exceeds the 5 MB limit */
        var content = new MemoryStream();
        content.SetLength((5 * 1024 * 1024) + 1);

        var model = new CustomersImportScheme
        {
            Content = content,
            FileName = "clientes.csv"
        };

        var submitted = false;
        var form = Render<CustomerImportForm>(parameters => parameters
            .Add(component => component.Model, model)
            .Add(component => component.OnValidSubmit, EventCallback.Factory.Create<CustomersImportScheme>(this, _ => submitted = true)));

        /* act: submit the invalid form */
        form.Find("form").Submit();

        /* assert: the submit was blocked and the size message surfaced */
        Assert.False(submitted);
        Assert.Contains("o arquivo deve ter no máximo 5 MB.", form.Markup);
    }

    [Fact(DisplayName = "[component] - when submitted with a valid csv within the limit, OnValidSubmit is invoked with the model")]
    public void When_SubmittedWithValidCsv_Then_OnValidSubmitIsInvokedWithTheModel()
    {
        /* arrange: a valid csv model within the size limit */
        var model = new CustomersImportScheme
        {
            Content = new MemoryStream([1, 2, 3]),
            FileName = "clientes.csv"
        };

        CustomersImportScheme? received = null;
        var form = Render<CustomerImportForm>(parameters => parameters
            .Add(component => component.Model, model)
            .Add(component => component.OnValidSubmit, EventCallback.Factory.Create<CustomersImportScheme>(this, scheme => received = scheme)));

        /* act: submit the valid form */
        form.Find("form").Submit();

        /* assert: the valid submit callback received the model */
        Assert.NotNull(received);
        Assert.Equal("clientes.csv", received.FileName);
    }

    [Fact(DisplayName = "[component] - when the cancel action is clicked, OnCancel is invoked")]
    public void When_CancelActionIsClicked_Then_OnCancelIsInvoked()
    {
        /* arrange: a form with a captured cancel */
        var cancelled = false;
        var form = Render<CustomerImportForm>(parameters => parameters
            .Add(component => component.OnCancel, EventCallback.Factory.Create(this, () => cancelled = true)));

        /* act: click the cancel button */
        form.FindAll("fluent-button")[1].Click();

        /* assert: the cancel callback was invoked */
        Assert.True(cancelled);
    }
}
