namespace Apta.Tests.Unit.Handlers;

public sealed class FilterCustomersHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<ICustomerRepository> _repository = new();
    private readonly Mock<IMapper> _mapper = new();

    [Fact(DisplayName = "when customers are filtered, a paginated success result with mapped items is returned")]
    public async Task When_HandleAsync_IsCalled_Then_PaginatedMappedResultIsReturned()
    {
        /* arrange: create parameters, matching customers and configure repository and mapper */
        var parameters = _fixture.Build<CustomerFiltersParameters>()
            .With(filters => filters.Order, SortDirection.Ascending.ToString())
            .With(filters => filters.Page, 2)
            .With(filters => filters.PageSize, 5)
            .Create();

        var customers = _fixture.CreateMany<Customer>(3)
            .ToList();

        _repository.Setup(repository => repository.FilterCustomersAsync(It.IsAny<CustomerFilters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(customers);

        _repository.Setup(repository => repository.CountCustomersAsync(It.IsAny<CustomerFilters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new System.Numerics.BigInteger(12));

        _mapper.Setup(mapper => mapper.Map<CustomerScheme>(It.IsAny<Customer>()))
            .Returns((Customer customer) => new CustomerScheme { Id = customer.Id, Name = customer.Name, Phone = customer.Phone, Document = customer.Document });

        var handler = new FilterCustomersHandler(_repository.Object, _mapper.Object);

        /* act: handle the filter request */
        var result = await handler.HandleAsync(parameters, TestContext.Current.CancellationToken);

        /* assert: verify success and mapped items */
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(3, result.Data.Items.Count);

        /* assert: verify pagination metadata */
        Assert.Equal(12, result.Data.Total);

        Assert.Equal(parameters.Page, result.Data.PageNumber);
        Assert.Equal(parameters.PageSize, result.Data.PageSize);

        _mapper.Verify(mapper => mapper.Map<CustomerScheme>(It.IsAny<Customer>()), Times.Exactly(3));
    }

    [Fact(DisplayName = "when no customers match, an empty paginated success result is returned")]
    public async Task When_HandleAsync_IsCalled_WithNoMatches_Then_EmptyPaginatedResultIsReturned()
    {
        /* arrange: create parameters and configure repository to return no customers */
        var parameters = _fixture.Build<CustomerFiltersParameters>()
            .With(filters => filters.Order, SortDirection.Descending.ToString())
            .With(filters => filters.Page, 1)
            .With(filters => filters.PageSize, 10)
            .Create();

        _repository.Setup(repository => repository.FilterCustomersAsync(It.IsAny<CustomerFilters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        _repository.Setup(repository => repository.CountCustomersAsync(It.IsAny<CustomerFilters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(System.Numerics.BigInteger.Zero);

        var handler = new FilterCustomersHandler(_repository.Object, _mapper.Object);

        /* act: handle the filter request */
        var result = await handler.HandleAsync(parameters, TestContext.Current.CancellationToken);

        /* assert: verify success and empty items */
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data.Items);

        /* assert: verify total and that the mapper is never called */
        Assert.Equal(0, result.Data.Total);

        _mapper.Verify(mapper => mapper.Map<CustomerScheme>(It.IsAny<Customer>()), Times.Never);
    }
}
