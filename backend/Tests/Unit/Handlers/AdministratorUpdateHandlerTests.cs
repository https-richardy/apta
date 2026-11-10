namespace Apta.Tests.Unit.Handlers;

public sealed class AdministratorUpdateHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IAdministratorRepository> _repository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IMapper> _mapper = new();

    [Fact(DisplayName = "when the administrator does not exist, failure result is returned")]
    public async Task When_HandleAsync_IsCalled_WithUnknownIdentifier_Then_FailureResultIsReturned()
    {
        /* arrange: create request and configure repository with no matching administrator */
        var request = _fixture.Create<UpdateAdministratorScheme>();

        _repository.Setup(repository => repository.FilterAdministratorsAsync(It.Is<AdministratorFilters>(filters => filters.Id != null), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var handler = new AdministratorUpdateHandler(_repository.Object, _unitOfWork.Object, _mapper.Object);

        /* act: handle the update request */
        var result = await handler.HandleAsync(request, TestContext.Current.CancellationToken);

        /* assert: verify failure result */
        Assert.False(result.IsSuccess);
        Assert.Equal(AdministratorErrors.DoesNotExist, result.Error);

        /* assert: verify no transaction and no persistence happen */
        _unitOfWork.Verify(unit => unit.TransactionAsync(It.IsAny<CancellationToken>(), It.IsAny<Func<CancellationToken, Task<AdministratorScheme>>>()), Times.Never);
        _repository.Verify(repository => repository.UpdateAsync(It.IsAny<Administrator>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "when another administrator already uses the same code, failure result is returned")]
    public async Task When_HandleAsync_IsCalled_WithCodeOwnedByAnotherAdministrator_Then_FailureResultIsReturned()
    {
        /* arrange: create request, the administrator being updated and another one sharing the target code */
        var request = _fixture.Create<UpdateAdministratorScheme>();
        var administrator = _fixture.Build<Administrator>()
            .With(entity => entity.Id, request.Id)
            .Create();

        var other = _fixture.Build<Administrator>()
            .With(entity => entity.Code, request.Code)
            .Create();

        _repository.Setup(repository => repository.FilterAdministratorsAsync(It.Is<AdministratorFilters>(filters => filters.Id != null), It.IsAny<CancellationToken>()))
            .ReturnsAsync([administrator]);

        _repository.Setup(repository => repository.FilterAdministratorsAsync(It.Is<AdministratorFilters>(filters => filters.Id == null && filters.Code != null), It.IsAny<CancellationToken>()))
            .ReturnsAsync([other]);

        var handler = new AdministratorUpdateHandler(_repository.Object, _unitOfWork.Object, _mapper.Object);

        /* act: handle the update request */
        var result = await handler.HandleAsync(request, TestContext.Current.CancellationToken);

        /* assert: verify failure result */
        Assert.False(result.IsSuccess);
        Assert.Equal(AdministratorErrors.AlreadyExists, result.Error);

        /* assert: verify no transaction and no persistence happen */
        _unitOfWork.Verify(unit => unit.TransactionAsync(It.IsAny<CancellationToken>(), It.IsAny<Func<CancellationToken, Task<AdministratorScheme>>>()), Times.Never);
        _repository.Verify(repository => repository.UpdateAsync(It.IsAny<Administrator>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "when the administrator exists and the code is free, it is updated and a success result is returned")]
    public async Task When_HandleAsync_IsCalled_WithExistingAdministrator_Then_AdministratorIsUpdatedAndSuccessResultIsReturned()
    {
        /* arrange: create request, the existing administrator and the expected response scheme */
        var request = _fixture.Create<UpdateAdministratorScheme>();
        var administrator = _fixture.Build<Administrator>()
            .With(entity => entity.Id, request.Id)
            .With(entity => entity.Code, request.Code)
            .Create();

        var scheme = _fixture.Create<AdministratorScheme>();

        _repository.Setup(repository => repository.FilterAdministratorsAsync(It.Is<AdministratorFilters>(filters => filters.Id != null), It.IsAny<CancellationToken>()))
            .ReturnsAsync([administrator]);

        _repository.Setup(repository => repository.FilterAdministratorsAsync(It.Is<AdministratorFilters>(filters => filters.Id == null && filters.Code != null), It.IsAny<CancellationToken>()))
            .ReturnsAsync([administrator]);

        _repository.Setup(repository => repository.UpdateAsync(administrator, It.IsAny<CancellationToken>()))
            .ReturnsAsync(administrator);

        _mapper.Setup(mapper => mapper.Map<AdministratorScheme>(administrator))
            .Returns(scheme);

        _unitOfWork.Setup(unit => unit.TransactionAsync(It.IsAny<CancellationToken>(), It.IsAny<Func<CancellationToken, Task<AdministratorScheme>>>()))
            .Returns((CancellationToken token, Func<CancellationToken, Task<AdministratorScheme>> action) => action(token));

        var handler = new AdministratorUpdateHandler(_repository.Object, _unitOfWork.Object, _mapper.Object);

        /* act: handle the update request */
        var result = await handler.HandleAsync(request, TestContext.Current.CancellationToken);

        /* assert: verify success result */
        Assert.True(result.IsSuccess);
        Assert.Equal(scheme, result.Data);

        /* assert: verify the request was mapped onto the existing administrator */
        _mapper.Verify(mapper => mapper.Map(request, administrator), Times.Once);

        /* assert: verify the administrator was persisted inside a transaction */
        _repository.Verify(repository => repository.UpdateAsync(administrator, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(unit => unit.TransactionAsync(It.IsAny<CancellationToken>(), It.IsAny<Func<CancellationToken, Task<AdministratorScheme>>>()), Times.Once);
    }
}
