namespace Apta.Tests.Unit.Handlers;

public sealed class AdministratorCreationHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IAdministratorRepository> _repository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IMapper> _mapper = new();

    [Fact(DisplayName = "when an administrator with the same code already exists, failure result is returned")]
    public async Task When_HandleAsync_IsCalled_WithExistingCode_Then_FailureResultIsReturned()
    {
        /* arrange: create request and an existing administrator sharing the same code */
        var request = _fixture.Create<AdministratorCreationScheme>();
        var existing = _fixture.Build<Administrator>()
            .With(entity => entity.Code, request.Code)
            .Create();

        _repository.Setup(repository => repository.FilterAdministratorsAsync(It.IsAny<AdministratorFilters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([existing]);

        var handler = new AdministratorCreationHandler(_repository.Object, _unitOfWork.Object, _mapper.Object);

        /* act: handle the creation request */
        var result = await handler.HandleAsync(request, TestContext.Current.CancellationToken);

        /* assert: verify failure result */
        Assert.False(result.IsSuccess);
        Assert.Equal(AdministratorErrors.AlreadyExists, result.Error);

        /* assert: verify no transaction and no persistence happen */
        _unitOfWork.Verify(unit => unit.TransactionAsync(It.IsAny<CancellationToken>(), It.IsAny<Func<CancellationToken, Task<AdministratorScheme>>>()), Times.Never);
        _repository.Verify(repository => repository.InsertAsync(It.IsAny<Administrator>(), It.IsAny<InsertionBehavior>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "when the administrator does not exist, it is persisted and a success result is returned")]
    public async Task When_HandleAsync_IsCalled_WithNewAdministrator_Then_AdministratorIsPersistedAndSuccessResultIsReturned()
    {
        /* arrange: create request, mapped entity and expected response scheme */
        var request = _fixture.Create<AdministratorCreationScheme>();
        var administrator = _fixture.Create<Administrator>();
        var scheme = _fixture.Create<AdministratorScheme>();

        _repository.Setup(repository => repository.FilterAdministratorsAsync(It.IsAny<AdministratorFilters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        _mapper.Setup(mapper => mapper.Map<Administrator>(request))
            .Returns(administrator);

        _repository.Setup(repository => repository.InsertAsync(administrator, It.IsAny<InsertionBehavior>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(administrator);

        _mapper.Setup(mapper => mapper.Map<AdministratorScheme>(administrator))
            .Returns(scheme);

        _unitOfWork.Setup(unit => unit.TransactionAsync(It.IsAny<CancellationToken>(), It.IsAny<Func<CancellationToken, Task<AdministratorScheme>>>()))
            .Returns((CancellationToken token, Func<CancellationToken, Task<AdministratorScheme>> action) => action(token));

        var handler = new AdministratorCreationHandler(_repository.Object, _unitOfWork.Object, _mapper.Object);

        /* act: handle the creation request */
        var result = await handler.HandleAsync(request, TestContext.Current.CancellationToken);

        /* assert: verify success result */
        Assert.True(result.IsSuccess);
        Assert.Equal(scheme, result.Data);

        /* assert: verify the administrator was persisted inside a transaction */
        _repository.Verify(repository => repository.InsertAsync(administrator, It.IsAny<InsertionBehavior>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(unit => unit.TransactionAsync(It.IsAny<CancellationToken>(), It.IsAny<Func<CancellationToken, Task<AdministratorScheme>>>()), Times.Once);
    }
}
