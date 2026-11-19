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

    [Fact(DisplayName = "when update customer is called and the update succeeds, 200 OK is returned with the customer")]
    public async Task When_UpdateCustomerAsync_IsCalled_WithSuccessfulUpdate_Then_OkIsReturned()
    {
        /* arrange: create the route identifier, request body and the dispatched success result */
        var id = _fixture.Create<string>();

        var request = _fixture.Create<UpdateCustomerScheme>();
        var scheme = _fixture.Create<CustomerScheme>();

        _dispatcher.Setup(dispatcher => dispatcher.DispatchAsync(It.Is<IDispatchable<Result<CustomerScheme>>>(dispatchable => ((UpdateCustomerScheme)dispatchable).Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CustomerScheme>.Success(scheme));

        var controller = new CustomersController(_dispatcher.Object);

        /* act: invoke the update endpoint */
        var response = await controller.UpdateCustomerAsync(id, request, TestContext.Current.CancellationToken);

        /* assert: verify the response is 200 OK carrying the customer */
        var result = Assert.IsType<ObjectResult>(response);

        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Equal(scheme, result.Value);
    }

    [Fact(DisplayName = "when update customer is called and the customer does not exist, 404 Not Found is returned with the error")]
    public async Task When_UpdateCustomerAsync_IsCalled_WithUnknownCustomer_Then_NotFoundIsReturned()
    {
        /* arrange: create the route identifier, request body and the dispatched failure result */
        var id = _fixture.Create<string>();
        var request = _fixture.Create<UpdateCustomerScheme>();

        _dispatcher.Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<IDispatchable<Result<CustomerScheme>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CustomerScheme>.Failure(CustomerErrors.DoesNotExist));

        var controller = new CustomersController(_dispatcher.Object);

        /* act: invoke the update endpoint */
        var response = await controller.UpdateCustomerAsync(id, request, TestContext.Current.CancellationToken);

        /* assert: verify the response is 404 Not Found carrying the error */
        var result = Assert.IsType<ObjectResult>(response);

        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.Equal(CustomerErrors.DoesNotExist, result.Value);
    }

    [Fact(DisplayName = "when update customer is called and the document already exists, 409 Conflict is returned with the error")]
    public async Task When_UpdateCustomerAsync_IsCalled_WithDuplicatedDocument_Then_ConflictIsReturned()
    {
        /* arrange: create the route identifier, request body and the dispatched failure result */
        var id = _fixture.Create<string>();
        var request = _fixture.Create<UpdateCustomerScheme>();

        _dispatcher.Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<IDispatchable<Result<CustomerScheme>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CustomerScheme>.Failure(CustomerErrors.AlreadyExists));

        var controller = new CustomersController(_dispatcher.Object);

        /* act: invoke the update endpoint */
        var response = await controller.UpdateCustomerAsync(id, request, TestContext.Current.CancellationToken);

        /* assert: verify the response is 409 Conflict carrying the error */
        var result = Assert.IsType<ObjectResult>(response);

        Assert.Equal(StatusCodes.Status409Conflict, result.StatusCode);
        Assert.Equal(CustomerErrors.AlreadyExists, result.Value);
    }

    [Fact(DisplayName = "when delete customer is called and the deletion succeeds, 204 No Content is returned")]
    public async Task When_DeleteCustomerAsync_IsCalled_WithSuccessfulDeletion_Then_NoContentIsReturned()
    {
        /* arrange: create the request and the dispatched success result */
        var request = _fixture.Create<CustomerDeletionScheme>();

        _dispatcher.Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<IDispatchable<Result>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var controller = new CustomersController(_dispatcher.Object);

        /* act: invoke the delete endpoint */
        var response = await controller.DeleteCustomerAsync(request, TestContext.Current.CancellationToken);

        /* assert: verify the response is 204 No Content */
        var result = Assert.IsType<StatusCodeResult>(response);

        Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
    }

    [Fact(DisplayName = "when delete customer is called and the customer does not exist, 404 Not Found is returned with the error")]
    public async Task When_DeleteCustomerAsync_IsCalled_WithUnknownCustomer_Then_NotFoundIsReturned()
    {
        /* arrange: create the request and the dispatched failure result */
        var request = _fixture.Create<CustomerDeletionScheme>();

        _dispatcher.Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<IDispatchable<Result>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(CustomerErrors.DoesNotExist));

        var controller = new CustomersController(_dispatcher.Object);

        /* act: invoke the delete endpoint */
        var response = await controller.DeleteCustomerAsync(request, TestContext.Current.CancellationToken);

        /* assert: verify the response is 404 Not Found carrying the error */
        var result = Assert.IsType<ObjectResult>(response);

        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.Equal(CustomerErrors.DoesNotExist, result.Value);
    }

    [Fact(DisplayName = "when import customers is called and the import is accepted, 202 Accepted is returned")]
    public async Task When_ImportCustomersAsync_IsCalled_WithAcceptedImport_Then_AcceptedIsReturned()
    {
        /* arrange: build the import request and the dispatched success result */
        var request = new CustomersImportScheme
        {
            File = new Blob { FileName = _fixture.Create<string>(), Stream = Stream.Null }
        };

        _dispatcher.Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<IDispatchable<Result>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var controller = new CustomersController(_dispatcher.Object);

        /* act: invoke the import endpoint */
        var response = await controller.ImportCustomersAsync(request, TestContext.Current.CancellationToken);

        /* assert: verify the response is 202 Accepted */
        var result = Assert.IsType<StatusCodeResult>(response);

        Assert.Equal(StatusCodes.Status202Accepted, result.StatusCode);
    }

    [Fact(DisplayName = "when import customers is called and the file is empty, 422 Unprocessable Entity is returned with the error")]
    public async Task When_ImportCustomersAsync_IsCalled_WithEmptyFile_Then_UnprocessableEntityIsReturned()
    {
        /* arrange: build the import request and the dispatched failure result */
        var request = new CustomersImportScheme
        {
            File = new Blob { FileName = _fixture.Create<string>(), Stream = Stream.Null }
        };

        _dispatcher.Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<IDispatchable<Result>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(CustomerErrors.NoFileContent));

        var controller = new CustomersController(_dispatcher.Object);

        /* act: invoke the import endpoint */
        var response = await controller.ImportCustomersAsync(request, TestContext.Current.CancellationToken);

        /* assert: verify the response is 422 Unprocessable Entity carrying the error */
        var result = Assert.IsType<ObjectResult>(response);

        Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.StatusCode);
        Assert.Equal(CustomerErrors.NoFileContent, result.Value);
    }
}
