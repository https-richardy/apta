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
}
