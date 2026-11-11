namespace Apta.Tests.Unit.Handlers;

public sealed class AdministratorDeletionHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IAdministratorRepository> _repository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    [Fact(DisplayName = "when the administrator does not exist, failure result is returned")]
    public async Task When_HandleAsync_IsCalled_WithUnknownIdentifier_Then_FailureResultIsReturned()
    {
        /* arrange: create request and configure repository with no matching administrator */
        var request = _fixture.Create<AdministratorDeletionScheme>();

        _repository.Setup(repository => repository.FilterAdministratorsAsync(It.Is<AdministratorFilters>(filters => filters.Id != null), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var handler = new AdministratorDeletionHandler(_repository.Object, _unitOfWork.Object);

        /* act: handle the deletion request */
        var result = await handler.HandleAsync(request, TestContext.Current.CancellationToken);

        /* assert: verify failure result */
        Assert.False(result.IsSuccess);
        Assert.Equal(AdministratorErrors.DoesNotExist, result.Error);

        /* assert: verify no transaction and no deletion happen */
        _unitOfWork.Verify(unit => unit.TransactionAsync(It.IsAny<CancellationToken>(), It.IsAny<Func<CancellationToken, Task>>()), Times.Never);
        _repository.Verify(repository => repository.DeleteAsync(It.IsAny<Administrator>(), It.IsAny<DeletionBehavior>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "when the administrator exists, it is deleted inside a transaction and a success result is returned")]
    public async Task When_HandleAsync_IsCalled_WithExistingAdministrator_Then_AdministratorIsDeletedAndSuccessResultIsReturned()
    {
        /* arrange: create request and the administrator being deleted */
        var request = _fixture.Create<AdministratorDeletionScheme>();
        var administrator = _fixture.Build<Administrator>()
            .With(entity => entity.Id, request.Id)
            .Create();

        _repository.Setup(repository => repository.FilterAdministratorsAsync(It.Is<AdministratorFilters>(filters => filters.Id != null), It.IsAny<CancellationToken>()))
            .ReturnsAsync([administrator]);

        _repository.Setup(repository => repository.DeleteAsync(administrator, It.IsAny<DeletionBehavior>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWork.Setup(unit => unit.TransactionAsync(It.IsAny<CancellationToken>(), It.IsAny<Func<CancellationToken, Task>>()))
            .Returns((CancellationToken token, Func<CancellationToken, Task> action) => action(token));

        var handler = new AdministratorDeletionHandler(_repository.Object, _unitOfWork.Object);

        /* act: handle the deletion request */
        var result = await handler.HandleAsync(request, TestContext.Current.CancellationToken);

        /* assert: verify success result */
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);

        /* assert: verify the administrator was deleted inside a transaction */
        _repository.Verify(repository => repository.DeleteAsync(administrator, It.IsAny<DeletionBehavior>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(unit => unit.TransactionAsync(It.IsAny<CancellationToken>(), It.IsAny<Func<CancellationToken, Task>>()), Times.Once);
    }
}
