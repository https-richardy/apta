namespace Apta.WebUI.Tests.Unit.Components.Administrators;

public sealed class AdministratorContentTests : BunitContext
{
    private readonly Fixture _fixture = new();

    public AdministratorContentTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddFluentUIComponents();
    }

    [Fact(DisplayName = "[component] - when the query state has an error, the error message is rendered")]
    public void When_StateHasError_Then_ErrorMessageIsRendered()
    {
        /* arrange: a failed query state without data */
        var state = QueryState<Pagination<AdministratorScheme>>.CreateFailure(new InvalidOperationException("boom"), default!);

        /* act: render the content with the failed state */
        var content = Render<AdministratorContent>(parameters => parameters
            .Add(component => component.State, state));

        /* assert: the error message bar is shown */
        Assert.Contains("Não foi possível carregar as administradoras", content.Markup);
    }

    [Fact(DisplayName = "[component] - when the query state carries administrators, the administrators table is rendered with them")]
    public void When_StateHasData_Then_AdministratorsTableIsRenderedWithThem()
    {
        /* arrange: a successful query state carrying a page of administrators */
        var administrators = _fixture.CreateMany<AdministratorScheme>(3)
            .ToList();

        var page = _fixture.Build<Pagination<AdministratorScheme>>()
            .With(pagination => pagination.Items, administrators)
            .Create();

        var state = QueryState<Pagination<AdministratorScheme>>.CreateSuccess(page, page);

        /* act: render the content with the successful state */
        var content = Render<AdministratorContent>(parameters => parameters
            .Add(component => component.State, state));

        /* assert: the table received the administrators from the page */
        var table = content.FindComponent<AdministratorsTable>();

        Assert.Equal(administrators.Count, table.Instance.Administrators.Count);
    }

    [Fact(DisplayName = "[component] - when the query state has an error, the message bar carries the error intent")]
    public void When_StateHasError_Then_MessageBarCarriesTheErrorIntent()
    {
        /* arrange: a failed query state without data */
        var state = QueryState<Pagination<AdministratorScheme>>.CreateFailure(new InvalidOperationException("boom"), default!);

        /* act: render the content with the failed state */
        var content = Render<AdministratorContent>(parameters => parameters
            .Add(component => component.State, state));

        /* assert: the message bar signals an error, not a neutral notice */
        var bar = content.FindComponent<FluentMessageBar>();

        Assert.Equal(MessageBarIntent.Error, bar.Instance.Intent);
    }

    [Fact(DisplayName = "[component] - when the query state carries administrators, the row action callbacks are forwarded to the table")]
    public void When_StateHasData_Then_RowActionCallbacksAreForwardedToTheTable()
    {
        /* arrange: a successful query state and the row action handlers wired on the content */
        var administrators = _fixture.CreateMany<AdministratorScheme>(3)
            .ToList();

        var page = _fixture.Build<Pagination<AdministratorScheme>>()
            .With(pagination => pagination.Items, administrators)
            .Create();

        var state = QueryState<Pagination<AdministratorScheme>>.CreateSuccess(page, page);

        /* act: render the content forwarding the update and delete handlers */
        var content = Render<AdministratorContent>(parameters => parameters
            .Add(component => component.State, state)
            .Add(component => component.OnUpdate, EventCallback.Factory.Create<AdministratorScheme>(this, _ => { }))
            .Add(component => component.OnDelete, EventCallback.Factory.Create<AdministratorScheme>(this, _ => { })));

        /* assert: the table received both callbacks */
        var table = content.FindComponent<AdministratorsTable>();

        Assert.True(table.Instance.OnUpdate.HasDelegate);
        Assert.True(table.Instance.OnDelete.HasDelegate);
    }

    [Fact(DisplayName = "[component] - when the query state has no data yet, a skeleton is rendered")]
    public void When_StateHasNoDataYet_Then_SkeletonIsRendered()
    {
        /* arrange: an idle state with no data loaded */
        var state = QueryState<Pagination<AdministratorScheme>>.CreateIdle(default!);

        /* act: render the content with the idle state */
        var content = Render<AdministratorContent>(parameters => parameters
            .Add(component => component.State, state));

        /* assert: the loading skeleton is shown */
        Assert.NotEmpty(content.FindComponents<Skeleton>());
    }
}
