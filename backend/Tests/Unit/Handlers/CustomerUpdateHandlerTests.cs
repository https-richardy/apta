namespace Apta.Tests.Unit.Handlers;

public sealed class CustomerUpdateHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<ICustomerRepository> _repository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IMapper> _mapper = new();

    [Fact(DisplayName = "when the customer does not exist, failure result is returned")]
    public async Task When_HandleAsync_IsCalled_WithUnknownIdentifier_Then_FailureResultIsReturned()
    {
        /* arrange: create request and configure repository with no matching customer */
        var request = _fixture.Create<UpdateCustomerScheme>();

        _repository.Setup(repository => repository.FilterCustomersAsync(It.Is<CustomerFilters>(filters => filters.Id != null), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var handler = new CustomerUpdateHandler(_repository.Object, _unitOfWork.Object, _mapper.Object);

        /* act: handle the update request */
        var result = await handler.HandleAsync(request, TestContext.Current.CancellationToken);

        /* assert: verify failure result */
        Assert.False(result.IsSuccess);
        Assert.Equal(CustomerErrors.DoesNotExist, result.Error);

        /* assert: verify no transaction and no persistence happen */
        _unitOfWork.Verify(unit => unit.TransactionAsync(It.IsAny<CancellationToken>(), It.IsAny<Func<CancellationToken, Task<CustomerScheme>>>()), Times.Never);
        _repository.Verify(repository => repository.UpdateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "when another customer already uses the same document, failure result is returned")]
    public async Task When_HandleAsync_IsCalled_WithDocumentOwnedByAnotherCustomer_Then_FailureResultIsReturned()
    {
        /* arrange: create request, the customer being updated and another one sharing the target document */
        var request = _fixture.Build<UpdateCustomerScheme>()
            .With(scheme => scheme.Document, "12345678909")
            .Create();

        var customer = _fixture.Build<Customer>()
            .With(entity => entity.Id, request.Id)
            .Create();

        var other = _fixture.Build<Customer>()
            .With(entity => entity.Document, "12345678909")
            .Create();

        _repository.Setup(repository => repository.FilterCustomersAsync(It.Is<CustomerFilters>(filters => filters.Id != null), It.IsAny<CancellationToken>()))
            .ReturnsAsync([customer]);

        _repository.Setup(repository => repository.FilterCustomersAsync(It.Is<CustomerFilters>(filters => filters.Id == null && filters.Document != null), It.IsAny<CancellationToken>()))
            .ReturnsAsync([other]);

        var handler = new CustomerUpdateHandler(_repository.Object, _unitOfWork.Object, _mapper.Object);

        /* act: handle the update request */
        var result = await handler.HandleAsync(request, TestContext.Current.CancellationToken);

        /* assert: verify failure result */
        Assert.False(result.IsSuccess);
        Assert.Equal(CustomerErrors.AlreadyExists, result.Error);

        /* assert: verify no transaction and no persistence happen */
        _unitOfWork.Verify(unit => unit.TransactionAsync(It.IsAny<CancellationToken>(), It.IsAny<Func<CancellationToken, Task<CustomerScheme>>>()), Times.Never);
        _repository.Verify(repository => repository.UpdateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "when the customer exists and the document is free, it is updated and a success result is returned")]
    public async Task When_HandleAsync_IsCalled_WithExistingCustomer_Then_CustomerIsUpdatedAndSuccessResultIsReturned()
    {
        /* arrange: create request, the existing customer and the expected response scheme */
        var request = _fixture.Build<UpdateCustomerScheme>()
            .With(scheme => scheme.Document, "12345678909")
            .Create();

        var customer = _fixture.Build<Customer>()
            .With(entity => entity.Id, request.Id)
            .With(entity => entity.Document, "12345678909")
            .Create();

        var scheme = _fixture.Create<CustomerScheme>();

        _repository.Setup(repository => repository.FilterCustomersAsync(It.Is<CustomerFilters>(filters => filters.Id != null), It.IsAny<CancellationToken>()))
            .ReturnsAsync([customer]);

        _repository.Setup(repository => repository.FilterCustomersAsync(It.Is<CustomerFilters>(filters => filters.Id == null && filters.Document != null), It.IsAny<CancellationToken>()))
            .ReturnsAsync([customer]);

        _repository.Setup(repository => repository.UpdateAsync(customer, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _mapper.Setup(mapper => mapper.Map<CustomerScheme>(customer))
            .Returns(scheme);

        _unitOfWork.Setup(unit => unit.TransactionAsync(It.IsAny<CancellationToken>(), It.IsAny<Func<CancellationToken, Task<CustomerScheme>>>()))
            .Returns((CancellationToken token, Func<CancellationToken, Task<CustomerScheme>> action) => action(token));

        var handler = new CustomerUpdateHandler(_repository.Object, _unitOfWork.Object, _mapper.Object);

        /* act: handle the update request */
        var result = await handler.HandleAsync(request, TestContext.Current.CancellationToken);

        /* assert: verify success result */
        Assert.True(result.IsSuccess);
        Assert.Equal(scheme, result.Data);

        /* assert: verify the request was mapped onto the existing customer */
        _mapper.Verify(mapper => mapper.Map(request, customer), Times.Once);

        /* assert: verify the customer was persisted inside a transaction */
        _repository.Verify(repository => repository.UpdateAsync(customer, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(unit => unit.TransactionAsync(It.IsAny<CancellationToken>(), It.IsAny<Func<CancellationToken, Task<CustomerScheme>>>()), Times.Once);
    }
}
