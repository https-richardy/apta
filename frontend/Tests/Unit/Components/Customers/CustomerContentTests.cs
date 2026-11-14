namespace Apta.WebUI.Tests.Unit.Components.Customers;

public sealed class CustomerContentTests : BunitContext
{
    private readonly Fixture _fixture = new();

    public CustomerContentTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddFluentUIComponents();
    }

    [Fact(DisplayName = "[component] - when the query state has an error, the error message is rendered")]
    public void When_StateHasError_Then_ErrorMessageIsRendered()
    {
        /* arrange: a failed query state without data */
        var state = QueryState<Pagination<CustomerScheme>>.CreateFailure(new InvalidOperationException("boom"), default!);

        /* act: render the content with the failed state */
        var content = Render<CustomerContent>(parameters => parameters
            .Add(component => component.State, state));

        /* assert: the error message bar is shown */
        Assert.Contains("Não foi possível carregar os clientes", content.Markup);
    }

    [Fact(DisplayName = "[component] - when the query state has an error, the message bar carries the error intent")]
    public void When_StateHasError_Then_MessageBarCarriesTheErrorIntent()
    {
        /* arrange: a failed query state without data */
        var state = QueryState<Pagination<CustomerScheme>>.CreateFailure(new InvalidOperationException("boom"), default!);

        /* act: render the content with the failed state */
        var content = Render<CustomerContent>(parameters => parameters
            .Add(component => component.State, state));

        /* assert: the message bar signals an error, not a neutral notice */
        var bar = content.FindComponent<FluentMessageBar>();

        Assert.Equal(MessageBarIntent.Error, bar.Instance.Intent);
    }

    [Fact(DisplayName = "[component] - when the query state carries customers, the customers table is rendered with them")]
    public void When_StateHasData_Then_CustomersTableIsRenderedWithThem()
    {
        /* arrange: a successful query state carrying a page of customers */
        var customers = _fixture.CreateMany<CustomerScheme>(3)
            .ToList();

        var page = _fixture.Build<Pagination<CustomerScheme>>()
            .With(pagination => pagination.Items, customers)
            .Create();

        var state = QueryState<Pagination<CustomerScheme>>.CreateSuccess(page, page);

        /* act: render the content with the successful state */
        var content = Render<CustomerContent>(parameters => parameters
            .Add(component => component.State, state));

        /* assert: the table received the customers from the page */
        var table = content.FindComponent<CustomersTable>();

        Assert.Equal(customers.Count, table.Instance.Customers.Count);
    }

    [Fact(DisplayName = "[component] - when the query state has no data yet, a skeleton is rendered")]
    public void When_StateHasNoDataYet_Then_SkeletonIsRendered()
    {
        /* arrange: an idle state with no data loaded */
        var state = QueryState<Pagination<CustomerScheme>>.CreateIdle(default!);

        /* act: render the content with the idle state */
        var content = Render<CustomerContent>(parameters => parameters
            .Add(component => component.State, state));

        /* assert: the loading skeleton is shown */
        Assert.NotEmpty(content.FindComponents<CustomersSkeleton>());
    }
}
