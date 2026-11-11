namespace Apta.Tests.Unit.Controllers;

public sealed class AdministratorsControllerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IDispatcher> _dispatcher = new();

    [Fact(DisplayName = "when update administrator is called and the update succeeds, 200 OK is returned with the administrator")]
    public async Task When_UpdateAdministratorAsync_IsCalled_WithSuccessfulUpdate_Then_OkIsReturned()
    {
        /* arrange: create the route identifier, request body and the dispatched success result */
        var id = _fixture.Create<string>();
        var request = _fixture.Create<UpdateAdministratorScheme>();
        var scheme = _fixture.Create<AdministratorScheme>();

        _dispatcher.Setup(dispatcher => dispatcher.DispatchAsync(It.Is<IDispatchable<Result<AdministratorScheme>>>(dispatchable => ((UpdateAdministratorScheme)dispatchable).Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AdministratorScheme>.Success(scheme));

        var controller = new AdministratorsController(_dispatcher.Object);

        /* act: invoke the update endpoint */
        var response = await controller.UpdateAdministratorAsync(id, request, TestContext.Current.CancellationToken);

        /* assert: verify the response is 200 OK carrying the administrator */
        var result = Assert.IsType<ObjectResult>(response);

        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Equal(scheme, result.Value);
    }

    [Fact(DisplayName = "when update administrator is called and the administrator does not exist, 404 Not Found is returned with the error")]
    public async Task When_UpdateAdministratorAsync_IsCalled_WithUnknownAdministrator_Then_NotFoundIsReturned()
    {
        /* arrange: create the route identifier, request body and the dispatched failure result */
        var id = _fixture.Create<string>();
        var request = _fixture.Create<UpdateAdministratorScheme>();

        _dispatcher.Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<IDispatchable<Result<AdministratorScheme>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AdministratorScheme>.Failure(AdministratorErrors.DoesNotExist));

        var controller = new AdministratorsController(_dispatcher.Object);

        /* act: invoke the update endpoint */
        var response = await controller.UpdateAdministratorAsync(id, request, TestContext.Current.CancellationToken);

        /* assert: verify the response is 404 Not Found carrying the error */
        var result = Assert.IsType<ObjectResult>(response);

        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.Equal(AdministratorErrors.DoesNotExist, result.Value);
    }

    [Fact(DisplayName = "when update administrator is called and the code already exists, 409 Conflict is returned with the error")]
    public async Task When_UpdateAdministratorAsync_IsCalled_WithDuplicatedCode_Then_ConflictIsReturned()
    {
        /* arrange: create the route identifier, request body and the dispatched failure result */
        var id = _fixture.Create<string>();
        var request = _fixture.Create<UpdateAdministratorScheme>();

        _dispatcher.Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<IDispatchable<Result<AdministratorScheme>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AdministratorScheme>.Failure(AdministratorErrors.AlreadyExists));

        var controller = new AdministratorsController(_dispatcher.Object);

        /* act: invoke the update endpoint */
        var response = await controller.UpdateAdministratorAsync(id, request, TestContext.Current.CancellationToken);

        /* assert: verify the response is 409 Conflict carrying the error */
        var result = Assert.IsType<ObjectResult>(response);

        Assert.Equal(StatusCodes.Status409Conflict, result.StatusCode);
        Assert.Equal(AdministratorErrors.AlreadyExists, result.Value);
    }

    [Fact(DisplayName = "when delete administrator is called and the deletion succeeds, 204 No Content is returned")]
    public async Task When_DeleteAdministratorAsync_IsCalled_WithSuccessfulDeletion_Then_NoContentIsReturned()
    {
        /* arrange: create the request and the dispatched success result */
        var request = _fixture.Create<AdministratorDeletionScheme>();

        _dispatcher.Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<IDispatchable<Result>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var controller = new AdministratorsController(_dispatcher.Object);

        /* act: invoke the delete endpoint */
        var response = await controller.DeleteAdministratorAsync(request, TestContext.Current.CancellationToken);

        /* assert: verify the response is 204 No Content */
        var result = Assert.IsType<StatusCodeResult>(response);

        Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
    }

    [Fact(DisplayName = "when delete administrator is called and the administrator does not exist, 404 Not Found is returned with the error")]
    public async Task When_DeleteAdministratorAsync_IsCalled_WithUnknownAdministrator_Then_NotFoundIsReturned()
    {
        /* arrange: create the request and the dispatched failure result */
        var request = _fixture.Create<AdministratorDeletionScheme>();

        _dispatcher.Setup(dispatcher => dispatcher.DispatchAsync(It.IsAny<IDispatchable<Result>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(AdministratorErrors.DoesNotExist));

        var controller = new AdministratorsController(_dispatcher.Object);

        /* act: invoke the delete endpoint */
        var response = await controller.DeleteAdministratorAsync(request, TestContext.Current.CancellationToken);

        /* assert: verify the response is 404 Not Found carrying the error */
        var result = Assert.IsType<ObjectResult>(response);

        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.Equal(AdministratorErrors.DoesNotExist, result.Value);
    }
}
