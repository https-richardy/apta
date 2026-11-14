namespace Apta.WebUI.Tests.Unit.Components.Customers;

public sealed class CustomerPageTests : BunitContext
{
    private readonly Fixture _fixture = new();
    private readonly Mock<ICustomersClient> _client = new();

    public CustomerPageTests()
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
        Services.AddScoped<CustomersService>();
    }

    [Fact(DisplayName = "[component] - when rendered, the loaded customers are shown through the content")]
    public void When_Rendered_Then_LoadedCustomersAreShown()
    {
        /* arrange: the client serves a page of customers */
        var customers = _fixture.CreateMany<CustomerScheme>(3)
            .ToList();

        var page = _fixture.Build<Pagination<CustomerScheme>>()
            .With(pagination => pagination.Items, customers)
            .Create();

        _client.Setup(client => client.GetCustomersAsync(It.IsAny<CustomerFiltersParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Pagination<CustomerScheme>>.Success(page));

        /* act: render the page */
        var component = Render<Pages.Customers.Customers>();

        /* assert: once the query settles, every customer is displayed */
        component.WaitForAssertion(() =>
            Assert.All(customers, customer => Assert.Contains(customer.Name, component.Markup)));
    }

    [Fact(DisplayName = "[component] - when the customers cannot be loaded, the error message is shown")]
    public void When_CustomersCannotBeLoaded_Then_ErrorMessageIsShown()
    {
        /* arrange: the client fails to serve the customers */
        _client.Setup(client => client.GetCustomersAsync(It.IsAny<CustomerFiltersParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Pagination<CustomerScheme>>.Failure(Error.Unknown));

        /* act: render the page */
        var component = Render<Pages.Customers.Customers>();

        /* assert: once the query settles, the error message bar is shown */
        component.WaitForAssertion(() =>
            Assert.Contains("Não foi possível carregar os clientes", component.Markup));
    }

    [Fact(DisplayName = "[component] - when the add action is clicked, the creation drawer is opened")]
    public void When_AddActionClicked_Then_CreationDrawerIsOpened()
    {
        /* arrange: the client serves a page of customers */
        var customers = _fixture.CreateMany<CustomerScheme>(3)
            .ToList();

        var page = _fixture.Build<Pagination<CustomerScheme>>()
            .With(pagination => pagination.Items, customers)
            .Create();

        _client.Setup(client => client.GetCustomersAsync(It.IsAny<CustomerFiltersParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Pagination<CustomerScheme>>.Success(page));

        var providers = Render<FluentProviders>();
        var component = Render<Pages.Customers.Customers>();

        /* act: click the add action */
        component.Find("fluent-button[appearance='primary']").Click();

        /* assert: the creation drawer is opened */
        providers.WaitForAssertion(() => Assert.Contains("Novo cliente", providers.Markup));
    }
}
