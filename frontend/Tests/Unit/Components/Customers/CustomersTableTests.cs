namespace Apta.WebUI.Tests.Unit.Components.Customers;

public sealed class CustomersTableTests : BunitContext
{
    private readonly Fixture _fixture = new();

    public CustomersTableTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddFluentUIComponents();
    }

    [Fact(DisplayName = "[component] - when customers are provided, each one is displayed in the table")]
    public void When_CustomersAreProvided_Then_EachOneIsDisplayed()
    {
        /* arrange: a handful of customers to display */
        var customers = _fixture.CreateMany<CustomerScheme>(3)
            .ToList();

        /* act: render the table with the customers */
        var table = Render<CustomersTable>(parameters => parameters
            .Add(component => component.Customers, customers));

        /* assert: every customer is displayed with its identifying information */
        Assert.All(customers, customer =>
        {
            Assert.Contains(customer.Name, table.Markup);
            Assert.Contains(customer.Id, table.Markup);

            Assert.Contains(customer.Phone, table.Markup);
            Assert.Contains(customer.Document, table.Markup);
        });
    }

    [Fact(DisplayName = "[component] - when the update action is clicked, OnUpdate is invoked with the row customer")]
    public void When_UpdateActionIsClicked_Then_OnUpdateIsInvokedWithTheRowCustomer()
    {
        /* arrange: a single customer and a captured callback */
        var customer = _fixture.Create<CustomerScheme>();

        CustomerScheme? updated = null;

        var table = Render<CustomersTable>(parameters => parameters
            .Add(component => component.Customers, [customer])
            .Add(component => component.OnUpdate, EventCallback.Factory.Create<CustomerScheme>(this, customer => updated = customer)));

        /* act: click the row update button */
        table.Find("fluent-button[title='Atualizar']").Click();

        /* assert: the callback received the row customer */
        Assert.Equal(customer, updated);
    }
}
