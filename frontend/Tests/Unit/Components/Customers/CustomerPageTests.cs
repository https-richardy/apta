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

    [Fact(DisplayName = "[component] - when the import action is clicked, the import dialog is opened")]
    public void When_ImportActionClicked_Then_ImportDialogIsOpened()
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

        /* act: click the import action */
        component.FindAll("fluent-button")
            .First(button => button.TextContent.Contains("Importar base"))
            .Click();

        /* assert: the import dialog is opened */
        providers.WaitForAssertion(() => Assert.Contains("Importar clientes", providers.Markup));
    }

    [Fact(DisplayName = "[component] - when a row is updated and the drawer is submitted, the customer is updated through the client")]
    public void When_RowUpdatedAndSubmitted_Then_CustomerIsUpdatedThroughTheClient()
    {
        /* arrange: the client serves customers (with a valid phone and document so the edit form submits without retyping) and accepts updates */
        var customers = _fixture.Build<CustomerScheme>()
            .With(customer => customer.Phone, "(11) 98888-7777")
            .With(customer => customer.Document, "123.456.789-09")
            .CreateMany(3)
            .ToList();

        var page = _fixture.Build<Pagination<CustomerScheme>>()
            .With(pagination => pagination.Items, customers)
            .Create();

        _client.Setup(client => client.GetCustomersAsync(It.IsAny<CustomerFiltersParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Pagination<CustomerScheme>>.Success(page));

        _client.Setup(client => client.UpdateCustomerAsync(It.IsAny<UpdateCustomerScheme>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CustomerScheme>.Success(customers[0]));

        var providers = Render<FluentProviders>();
        var component = Render<Pages.Customers.Customers>();

        component.WaitForState(() => component.FindAll("fluent-button[title='Atualizar']").Count > 0);

        /* act: open the update drawer for the first row and submit it */
        component.Find("fluent-button[title='Atualizar']").Click();

        providers.WaitForState(() => providers.Markup.Contains("Atualizar cliente"));
        providers.Find("form").Submit();

        /* assert: the first customer was updated through the client */
        component.WaitForAssertion(() => _client.Verify(client => client.UpdateCustomerAsync(
                It.Is<UpdateCustomerScheme>(scheme => scheme.Id == customers[0].Id),
                It.IsAny<CancellationToken>()), Times.Once)
        );
    }

    [Fact(DisplayName = "[component] - when the delete action is confirmed, the customer is deleted through the client")]
    public void When_DeleteConfirmed_Then_CustomerIsDeletedThroughTheClient()
    {
        /* arrange: the client serves customers and accepts deletions */
        var customers = _fixture.CreateMany<CustomerScheme>(3)
            .ToList();

        var page = _fixture.Build<Pagination<CustomerScheme>>()
            .With(pagination => pagination.Items, customers)
            .Create();

        _client.Setup(client => client.GetCustomersAsync(It.IsAny<CustomerFiltersParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Pagination<CustomerScheme>>.Success(page));

        _client.Setup(client => client.DeleteCustomerAsync(It.IsAny<CustomerDeletionScheme>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var providers = Render<FluentProviders>();
        var component = Render<Pages.Customers.Customers>();

        component.WaitForState(() => component.FindAll("fluent-button[title='Excluir']").Count > 0);
        component.Find("fluent-button[title='Excluir']").Click();

        /* act: confirm the deletion of the first row */
        providers.WaitForState(() => providers.Markup.Contains("Excluir cliente"));
        providers.FindAll("fluent-button").First(button => button.TextContent.Contains("Excluir")).Click();

        /* assert: the first customer was deleted through the client */
        component.WaitForAssertion(() => _client.Verify(client => client.DeleteCustomerAsync(
                It.Is<CustomerDeletionScheme>(scheme => scheme.Id == customers[0].Id),
                It.IsAny<CancellationToken>()), Times.Once)
        );
    }

    [Fact(DisplayName = "[component] - when the delete action is dismissed, no deletion is performed")]
    public void When_DeleteDismissed_Then_NoDeletionIsPerformed()
    {
        /* arrange: the client serves customers */
        var customers = _fixture.CreateMany<CustomerScheme>(3)
            .ToList();

        var page = _fixture.Build<Pagination<CustomerScheme>>()
            .With(pagination => pagination.Items, customers)
            .Create();

        _client.Setup(client => client.GetCustomersAsync(It.IsAny<CustomerFiltersParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Pagination<CustomerScheme>>.Success(page));

        var providers = Render<FluentProviders>();
        var component = Render<Pages.Customers.Customers>();

        component.WaitForState(() => component.FindAll("fluent-button[title='Excluir']").Count > 0);
        component.Find("fluent-button[title='Excluir']").Click();

        /* act: dismiss the confirmation for the first row */
        providers.WaitForState(() => providers.Markup.Contains("Excluir cliente"));
        providers.FindAll("fluent-button").First(button => button.TextContent.Contains("Cancelar")).Click();

        /* assert: nothing was deleted */
        _client.Verify(client => client.DeleteCustomerAsync(It.IsAny<CustomerDeletionScheme>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
