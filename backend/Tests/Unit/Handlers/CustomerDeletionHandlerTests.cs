namespace Apta.Tests.Unit.Handlers;

public sealed class CustomerDeletionHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<ICustomerRepository> _repository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    [Fact(DisplayName = "when the customer does not exist, failure result is returned")]
    public async Task When_HandleAsync_IsCalled_WithUnknownIdentifier_Then_FailureResultIsReturned()
    {
        /* arrange: create request and configure repository with no matching customer */
        var request = _fixture.Create<CustomerDeletionScheme>();

        _repository.Setup(repository => repository.FilterCustomersAsync(It.Is<CustomerFilters>(filters => filters.Id != null), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var handler = new CustomerDeletionHandler(_repository.Object, _unitOfWork.Object);

        /* act: handle the deletion request */
        var result = await handler.HandleAsync(request, TestContext.Current.CancellationToken);

        /* assert: verify failure result */
        Assert.False(result.IsSuccess);
        Assert.Equal(CustomerErrors.DoesNotExist, result.Error);

        /* assert: verify no transaction and no deletion happen */
        _unitOfWork.Verify(unit => unit.TransactionAsync(It.IsAny<CancellationToken>(), It.IsAny<Func<CancellationToken, Task>>()), Times.Never);
        _repository.Verify(repository => repository.DeleteAsync(It.IsAny<Customer>(), It.IsAny<DeletionBehavior>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "when the customer exists, it is deleted inside a transaction and a success result is returned")]
    public async Task When_HandleAsync_IsCalled_WithExistingCustomer_Then_CustomerIsDeletedAndSuccessResultIsReturned()
    {
        /* arrange: create request and the customer being deleted */
        var request = _fixture.Create<CustomerDeletionScheme>();
        var customer = _fixture.Build<Customer>()
            .With(customer => customer.Id, request.Id)
            .Create();

        _repository.Setup(repository => repository.FilterCustomersAsync(It.Is<CustomerFilters>(filters => filters.Id != null), It.IsAny<CancellationToken>()))
            .ReturnsAsync([customer]);

        _repository.Setup(repository => repository.DeleteAsync(customer, It.IsAny<DeletionBehavior>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWork.Setup(unit => unit.TransactionAsync(It.IsAny<CancellationToken>(), It.IsAny<Func<CancellationToken, Task>>()))
            .Returns((CancellationToken token, Func<CancellationToken, Task> action) => action(token));

        var handler = new CustomerDeletionHandler(_repository.Object, _unitOfWork.Object);

        /* act: handle the deletion request */
        var result = await handler.HandleAsync(request, TestContext.Current.CancellationToken);

        /* assert: verify success result */
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);

        /* assert: verify the customer was deleted inside a transaction */
        _repository.Verify(repository => repository.DeleteAsync(customer, It.IsAny<DeletionBehavior>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(unit => unit.TransactionAsync(It.IsAny<CancellationToken>(), It.IsAny<Func<CancellationToken, Task>>()), Times.Once);
    }
}
