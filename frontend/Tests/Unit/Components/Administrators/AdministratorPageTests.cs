namespace Apta.WebUI.Tests.Unit.Components.Administrators;

public sealed class AdministratorPageTests : BunitContext
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IAdministratorsClient> _client = new();

    public AdministratorPageTests()
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
        Services.AddScoped<AdministratorsService>();
    }

    [Fact(DisplayName = "[component] - when rendered, the loaded administrators are shown through the content")]
    public void When_Rendered_Then_LoadedAdministratorsAreShown()
    {
        /* arrange: the client serves a page of administrators */
        var administrators = _fixture.Build<AdministratorScheme>()
            .With(administrator => administrator.Code, "58113812")
            .CreateMany(3)
            .ToList();

        var page = _fixture.Build<Pagination<AdministratorScheme>>()
            .With(pagination => pagination.Items, administrators)
            .Create();

        _client.Setup(client => client.GetAdministratorsAsync(It.IsAny<AdministratorFiltersParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Pagination<AdministratorScheme>>.Success(page));

        /* act: render the page */
        var component = Render<Pages.Administrators.Administrators>();

        /* assert: once the query settles, every administrator is displayed */
        component.WaitForAssertion(() =>
            Assert.All(administrators, administrator => Assert.Contains(administrator.Name, component.Markup)));
    }

    [Fact(DisplayName = "[component] - when the add action is clicked, the creation drawer is opened")]
    public void When_AddActionClicked_Then_CreationDrawerIsOpened()
    {
        /* arrange: the client serves a page of administrators */
        var administrators = _fixture.Build<AdministratorScheme>()
            .With(administrator => administrator.Code, "58113812")
            .CreateMany(3)
            .ToList();

        var page = _fixture.Build<Pagination<AdministratorScheme>>()
            .With(pagination => pagination.Items, administrators)
            .Create();

        _client.Setup(client => client.GetAdministratorsAsync(It.IsAny<AdministratorFiltersParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Pagination<AdministratorScheme>>.Success(page));

        var providers = Render<FluentProviders>();
        var component = Render<Pages.Administrators.Administrators>();

        /* act: click the add action */
        component.Find("fluent-button[appearance='primary']").Click();
        providers.WaitForAssertion(() => Assert.Contains("Nova administradora", providers.Markup));
    }

    [Fact(DisplayName = "[component] - when a row is updated and the drawer is submitted, the administrator is updated through the client")]
    public void When_RowUpdatedAndSubmitted_Then_AdministratorIsUpdatedThroughTheClient()
    {
        /* arrange: the client serves administrators (with a valid code so the edit form submits without retyping) and accepts updates */
        var administrators = _fixture.Build<AdministratorScheme>()
            .With(administrator => administrator.Code, "58113812")
            .CreateMany(3)
            .ToList();

        var page = _fixture.Build<Pagination<AdministratorScheme>>()
            .With(pagination => pagination.Items, administrators)
            .Create();

        _client.Setup(client => client.GetAdministratorsAsync(It.IsAny<AdministratorFiltersParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Pagination<AdministratorScheme>>.Success(page));

        _client.Setup(client => client.UpdateAdministratorAsync(It.IsAny<UpdateAdministratorScheme>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AdministratorScheme>.Success(administrators[0]));

        var providers = Render<FluentProviders>();
        var component = Render<Pages.Administrators.Administrators>();

        component.WaitForState(() => component.FindAll("fluent-button[title='Atualizar']").Count > 0);

        /* act: open the update drawer for the first row and submit it */
        component.Find("fluent-button[title='Atualizar']").Click();

        providers.WaitForState(() => providers.Markup.Contains("Atualizar administradora"));
        providers.Find("form").Submit();

        /* assert: the first administrator was updated through the client */
        component.WaitForAssertion(() => _client.Verify(client => client.UpdateAdministratorAsync(
                It.Is<UpdateAdministratorScheme>(scheme => scheme.Id == administrators[0].Id),
                It.IsAny<CancellationToken>()), Times.Once)
        );
    }

    [Fact(DisplayName = "[component] - when the delete action is confirmed, the administrator is deleted through the client")]
    public void When_DeleteConfirmed_Then_AdministratorIsDeletedThroughTheClient()
    {
        /* arrange: the client serves administrators and accepts deletions */
        var administrators = _fixture.Build<AdministratorScheme>()
            .With(administrator => administrator.Code, "58113812")
            .CreateMany(3)
            .ToList();

        var page = _fixture.Build<Pagination<AdministratorScheme>>()
            .With(pagination => pagination.Items, administrators)
            .Create();

        _client.Setup(client => client.GetAdministratorsAsync(It.IsAny<AdministratorFiltersParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Pagination<AdministratorScheme>>.Success(page));

        _client.Setup(client => client.DeleteAdministratorAsync(It.IsAny<AdministratorDeletionScheme>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var providers = Render<FluentProviders>();
        var component = Render<Pages.Administrators.Administrators>();

        component.WaitForState(() => component.FindAll("fluent-button[title='Excluir']").Count > 0);
        component.Find("fluent-button[title='Excluir']").Click();

        providers.WaitForState(() => providers.Markup.Contains("Excluir administradora"));
        providers.FindAll("fluent-button").First(button => button.TextContent.Contains("Excluir")).Click();

        /* assert: the first administrator was deleted through the client */
        component.WaitForAssertion(() => _client.Verify(client => client.DeleteAdministratorAsync(
                It.Is<AdministratorDeletionScheme>(scheme => scheme.Id == administrators[0].Id),
                It.IsAny<CancellationToken>()), Times.Once)
        );
    }

    [Fact(DisplayName = "[component] - when the delete action is dismissed, no deletion is performed")]
    public void When_DeleteDismissed_Then_NoDeletionIsPerformed()
    {
        /* arrange: the client serves administrators */
        var administrators = _fixture.Build<AdministratorScheme>()
            .With(administrator => administrator.Code, "58113812")
            .CreateMany(3)
            .ToList();

        var page = _fixture.Build<Pagination<AdministratorScheme>>()
            .With(pagination => pagination.Items, administrators)
            .Create();

        _client.Setup(client => client.GetAdministratorsAsync(It.IsAny<AdministratorFiltersParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Pagination<AdministratorScheme>>.Success(page));

        var providers = Render<FluentProviders>();
        var component = Render<Pages.Administrators.Administrators>();

        component.WaitForState(() => component.FindAll("fluent-button[title='Excluir']").Count > 0);
        component.Find("fluent-button[title='Excluir']").Click();

        providers.WaitForState(() => providers.Markup.Contains("Excluir administradora"));
        providers.FindAll("fluent-button").First(button => button.TextContent.Contains("Cancelar")).Click();

        /* assert: nothing was deleted */
        _client.Verify(client => client.DeleteAdministratorAsync(It.IsAny<AdministratorDeletionScheme>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
