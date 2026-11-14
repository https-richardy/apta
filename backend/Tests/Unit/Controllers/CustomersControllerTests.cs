namespace Apta.Tests.Unit.Controllers;

public sealed class CustomersControllerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IDispatcher> _dispatcher = new();

    [Fact(DisplayName = "when filter customers is called, 200 OK is returned with the paginated customers")]
    public async Task When_FilterCustomersAsync_IsCalled_Then_OkIsReturned()
    {
        /* arrange: create the request and the dispatched paginated success result */
        var request = _fixture.Create<CustomerFiltersParameters>();
        var pagination = _fixture.Create<Pagination<CustomerScheme>>();

        _dispatcher.Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<IDispatchable<Result<Pagination<CustomerScheme>>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Pagination<CustomerScheme>>.Success(pagination));

        var controller = new CustomersController(_dispatcher.Object);

        /* act: invoke the filter endpoint */
        var response = await controller.FilterCustomersAsync(request, TestContext.Current.CancellationToken);

        /* assert: verify the response is 200 OK carrying the paginated customers */
        var result = Assert.IsType<ObjectResult>(response);

        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Equal(pagination, result.Value);
    }

    [Fact(DisplayName = "when create customer is called and the creation succeeds, 201 Created is returned with the customer")]
    public async Task When_CreateCustomerAsync_IsCalled_WithSuccessfulCreation_Then_CreatedIsReturned()
    {
        /* arrange: create the request body and the dispatched success result */
        var request = _fixture.Create<CustomerCreationScheme>();
        var scheme = _fixture.Create<CustomerScheme>();

        _dispatcher.Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<IDispatchable<Result<CustomerScheme>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CustomerScheme>.Success(scheme));

        var controller = new CustomersController(_dispatcher.Object);

        /* act: invoke the create endpoint */
        var response = await controller.CreateCustomerAsync(request, TestContext.Current.CancellationToken);

        /* assert: verify the response is 201 Created carrying the customer */
        var result = Assert.IsType<ObjectResult>(response);

        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal(scheme, result.Value);
    }

    [Fact(DisplayName = "when create customer is called and the document already exists, 409 Conflict is returned with the error")]
    public async Task When_CreateCustomerAsync_IsCalled_WithDuplicatedDocument_Then_ConflictIsReturned()
    {
        /* arrange: create the request body and the dispatched failure result */
        var request = _fixture.Create<CustomerCreationScheme>();

        _dispatcher.Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<IDispatchable<Result<CustomerScheme>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CustomerScheme>.Failure(CustomerErrors.AlreadyExists));

        var controller = new CustomersController(_dispatcher.Object);

        /* act: invoke the create endpoint */
        var response = await controller.CreateCustomerAsync(request, TestContext.Current.CancellationToken);

        /* assert: verify the response is 409 Conflict carrying the error */
        var result = Assert.IsType<ObjectResult>(response);

        Assert.Equal(StatusCodes.Status409Conflict, result.StatusCode);
        Assert.Equal(CustomerErrors.AlreadyExists, result.Value);
    }
}
