namespace Apta.Tests.Unit.Handlers;

public sealed class CustomerCreationHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<ICustomerRepository> _repository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IMapper> _mapper = new();

    [Fact(DisplayName = "when a customer with the same document already exists, failure result is returned")]
    public async Task When_HandleAsync_IsCalled_WithExistingDocument_Then_FailureResultIsReturned()
    {
        /* arrange: create request and an existing customer sharing the same document */
        var request = _fixture.Create<CustomerCreationScheme>();
        var existing = _fixture.Build<Customer>()
            .With(entity => entity.Document, request.Document!)
            .Create();

        _repository.Setup(repository => repository.FilterCustomersAsync(It.IsAny<CustomerFilters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([existing]);

        var handler = new CustomerCreationHandler(_repository.Object, _unitOfWork.Object, _mapper.Object);

        /* act: handle the creation request */
        var result = await handler.HandleAsync(request, TestContext.Current.CancellationToken);

        /* assert: verify failure result */
        Assert.False(result.IsSuccess);
        Assert.Equal(CustomerErrors.AlreadyExists, result.Error);

        /* assert: verify no transaction and no persistence happen */
        _unitOfWork.Verify(unit => unit.TransactionAsync(It.IsAny<CancellationToken>(), It.IsAny<Func<CancellationToken, Task<CustomerScheme>>>()), Times.Never);
        _repository.Verify(repository => repository.InsertAsync(It.IsAny<Customer>(), It.IsAny<InsertionBehavior>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "when the customer does not exist, it is persisted and a success result is returned")]
    public async Task When_HandleAsync_IsCalled_WithNewCustomer_Then_CustomerIsPersistedAndSuccessResultIsReturned()
    {
        /* arrange: create request, mapped entity and expected response scheme */
        var request = _fixture.Create<CustomerCreationScheme>();
        var customer = _fixture.Create<Customer>();
        var scheme = _fixture.Create<CustomerScheme>();

        _repository.Setup(repository => repository.FilterCustomersAsync(It.IsAny<CustomerFilters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        _mapper.Setup(mapper => mapper.Map<Customer>(request))
            .Returns(customer);

        _repository.Setup(repository => repository.InsertAsync(customer, It.IsAny<InsertionBehavior>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _mapper.Setup(mapper => mapper.Map<CustomerScheme>(customer))
            .Returns(scheme);

        _unitOfWork.Setup(unit => unit.TransactionAsync(It.IsAny<CancellationToken>(), It.IsAny<Func<CancellationToken, Task<CustomerScheme>>>()))
            .Returns((CancellationToken token, Func<CancellationToken, Task<CustomerScheme>> action) => action(token));

        var handler = new CustomerCreationHandler(_repository.Object, _unitOfWork.Object, _mapper.Object);

        /* act: handle the creation request */
        var result = await handler.HandleAsync(request, TestContext.Current.CancellationToken);

        /* assert: verify success result */
        Assert.True(result.IsSuccess);
        Assert.Equal(scheme, result.Data);

        /* assert: verify the customer was persisted inside a transaction */
        _repository.Verify(repository => repository.InsertAsync(customer, It.IsAny<InsertionBehavior>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(unit => unit.TransactionAsync(It.IsAny<CancellationToken>(), It.IsAny<Func<CancellationToken, Task<CustomerScheme>>>()), Times.Once);
    }

    [Fact(DisplayName = "when the customer has no document, the duplicate check is skipped and it is persisted")]
    public async Task When_HandleAsync_IsCalled_WithoutDocument_Then_DuplicateCheckIsSkippedAndCustomerIsPersisted()
    {
        /* arrange: create request without a document and configure the mapper and repository */
        var request = _fixture.Build<CustomerCreationScheme>()
            .Without(customer => customer.Document)
            .Create();

        var customer = _fixture.Create<Customer>();
        var scheme = _fixture.Create<CustomerScheme>();

        _mapper.Setup(mapper => mapper.Map<Customer>(request))
            .Returns(customer);

        _repository.Setup(repository => repository.InsertAsync(customer, It.IsAny<InsertionBehavior>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _mapper.Setup(mapper => mapper.Map<CustomerScheme>(customer))
            .Returns(scheme);

        _unitOfWork.Setup(unit => unit.TransactionAsync(It.IsAny<CancellationToken>(), It.IsAny<Func<CancellationToken, Task<CustomerScheme>>>()))
            .Returns((CancellationToken token, Func<CancellationToken, Task<CustomerScheme>> action) => action(token));

        var handler = new CustomerCreationHandler(_repository.Object, _unitOfWork.Object, _mapper.Object);

        /* act: handle the creation request */
        var result = await handler.HandleAsync(request, TestContext.Current.CancellationToken);

        /* assert: verify success result */
        Assert.True(result.IsSuccess);
        Assert.Equal(scheme, result.Data);

        /* assert: verify the duplicate check never queried the repository */
        _repository.Verify(repository => repository.FilterCustomersAsync(It.IsAny<CustomerFilters>(), It.IsAny<CancellationToken>()), Times.Never);
        _repository.Verify(repository => repository.InsertAsync(customer, It.IsAny<InsertionBehavior>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
