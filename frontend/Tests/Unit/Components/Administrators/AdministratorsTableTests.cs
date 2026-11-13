namespace Apta.WebUI.Tests.Unit.Components.Administrators;

public sealed class AdministratorsTableTests : BunitContext
{
    private readonly Fixture _fixture = new();

    public AdministratorsTableTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddFluentUIComponents();
    }

    [Fact(DisplayName = "[component] - when administrators are provided, each one is displayed in the table")]
    public void When_AdministratorsAreProvided_Then_EachOneIsDisplayed()
    {
        /* arrange: a handful of administrators to display */
        var administrators = _fixture.CreateMany<AdministratorScheme>(3)
            .ToList();

        /* act: render the table with the administrators */
        var table = Render<AdministratorsTable>(parameters => parameters
            .Add(component => component.Administrators, administrators));

        /* assert: every administrator is displayed with its identifying information */
        Assert.All(administrators, administrator =>
        {
            Assert.Contains(administrator.Name, table.Markup);
            Assert.Contains(administrator.Code, table.Markup);
            Assert.Contains(administrator.Id, table.Markup);
        });
    }

    [Fact(DisplayName = "[component] - when the update action is clicked, OnUpdate is invoked with the row administrator")]
    public void When_UpdateActionIsClicked_Then_OnUpdateIsInvokedWithTheRowAdministrator()
    {
        /* arrange: a single administrator and a captured callback */
        var administrator = _fixture.Create<AdministratorScheme>();

        AdministratorScheme? updated = null;

        var table = Render<AdministratorsTable>(parameters => parameters
            .Add(component => component.Administrators, [administrator])
            .Add(component => component.OnUpdate, EventCallback.Factory.Create<AdministratorScheme>(this, administrator => updated = administrator)));

        /* act: click the row update button */
        table.Find("fluent-button[title='Atualizar']").Click();

        /* assert: the callback received the row administrator */
        Assert.Equal(administrator, updated);
    }

    [Fact(DisplayName = "[component] - when the delete action is clicked, OnDelete is invoked with the row administrator")]
    public void When_DeleteActionIsClicked_Then_OnDeleteIsInvokedWithTheRowAdministrator()
    {
        /* arrange: a single administrator and a captured callback */
        var administrator = _fixture.Create<AdministratorScheme>();

        AdministratorScheme? deleted = null;

        var table = Render<AdministratorsTable>(parameters => parameters
            .Add(component => component.Administrators, [administrator])
            .Add(component => component.OnDelete, EventCallback.Factory.Create<AdministratorScheme>(this, administrator => deleted = administrator)));

        /* act: click the row delete button */
        table.Find("fluent-button[title='Excluir']").Click();

        /* assert: the callback received the row administrator */
        Assert.Equal(administrator, deleted);
    }
}
