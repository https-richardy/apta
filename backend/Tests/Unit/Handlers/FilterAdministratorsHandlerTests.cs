namespace Apta.Tests.Unit.Handlers;

public sealed class FilterAdministratorsHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IAdministratorRepository> _repository = new();
    private readonly Mock<IMapper> _mapper = new();

    [Fact(DisplayName = "when administrators are filtered, a paginated success result with mapped items is returned")]
    public async Task When_HandleAsync_IsCalled_Then_PaginatedMappedResultIsReturned()
    {
        /* arrange: create parameters, matching administrators and configure repository and mapper */
        var parameters = _fixture.Build<AdministratorFiltersParameters>()
            .With(filters => filters.Order, SortDirection.Ascending.ToString())
            .With(filters => filters.Page, 2)
            .With(filters => filters.PageSize, 5)
            .Create();

        var administrators = _fixture.CreateMany<Administrator>(3).ToList();

        _repository.Setup(repository => repository.FilterAdministratorsAsync(It.IsAny<AdministratorFilters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(administrators);

        _repository.Setup(repository => repository.CountAdministratorsAsync(It.IsAny<AdministratorFilters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new System.Numerics.BigInteger(12));

        _mapper.Setup(mapper => mapper.Map<AdministratorScheme>(It.IsAny<Administrator>()))
            .Returns((Administrator administrator) => new AdministratorScheme { Id = administrator.Id, Name = administrator.Name, Code = administrator.Code });

        var handler = new FilterAdministratorsHandler(_repository.Object, _mapper.Object);

        /* act: handle the filter request */
        var result = await handler.HandleAsync(parameters, TestContext.Current.CancellationToken);

        /* assert: verify success and mapped items */
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(3, result.Data.Items.Count);

        /* assert: verify pagination metadata */
        Assert.Equal(12, result.Data.Total);
        Assert.Equal(parameters.Page, result.Data.PageNumber);

        /* assert: verify page size and mapping interactions */
        Assert.Equal(parameters.PageSize, result.Data.PageSize);

        _mapper.Verify(mapper => mapper.Map<AdministratorScheme>(It.IsAny<Administrator>()), Times.Exactly(3));
    }

    [Fact(DisplayName = "when no administrators match, an empty paginated success result is returned")]
    public async Task When_HandleAsync_IsCalled_WithNoMatches_Then_EmptyPaginatedResultIsReturned()
    {
        /* arrange: create parameters and configure repository to return no administrators */
        var parameters = _fixture.Build<AdministratorFiltersParameters>()
            .With(filters => filters.Order, SortDirection.Descending.ToString())
            .With(filters => filters.Page, 1)
            .With(filters => filters.PageSize, 10)
            .Create();

        _repository.Setup(repository => repository.FilterAdministratorsAsync(It.IsAny<AdministratorFilters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        _repository.Setup(repository => repository.CountAdministratorsAsync(It.IsAny<AdministratorFilters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(System.Numerics.BigInteger.Zero);

        var handler = new FilterAdministratorsHandler(_repository.Object, _mapper.Object);

        /* act: handle the filter request */
        var result = await handler.HandleAsync(parameters, TestContext.Current.CancellationToken);

        /* assert: verify success and empty items */
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data.Items);

        /* assert: verify total and that the mapper is never called */
        Assert.Equal(0, result.Data.Total);

        _mapper.Verify(mapper => mapper.Map<AdministratorScheme>(It.IsAny<Administrator>()), Times.Never);
    }
}
