namespace Apta.Tests.Integration.Persistence;

[Collection(nameof(DisableParallelizationCollection))]
public sealed class AdministratorRepositoryTests(PostgresDatabase postgres) :
    IClassFixture<PostgresDatabase>,
    IAsyncLifetime
{
    private readonly Fixture _fixture = new();

    [Fact(DisplayName = "[integration] - when 'filter administrators async' is called without filters, all administrators are returned")]
    public async Task When_FilterAdministratorsAsync_IsCalled_WithoutFilters_Then_AllAdministratorsAreReturned()
    {
        /* arrange: insert administrators into postgres */
        var administrators = _fixture.Build<Administrator>()
            .With(entity => entity.Code, () => Guid.NewGuid().ToString("N")[..8])
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .CreateMany(2);

        await postgres.Context.Administrators.AddRangeAsync(administrators, TestContext.Current.CancellationToken);
        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new AdministratorRepository(postgres.Context, NullLogger<AdministratorRepository>.Instance);

        /* act: query administrators */
        var result = await repository.FilterAdministratorsAsync(AdministratorFilters.WithoutFilters, TestContext.Current.CancellationToken);

        /* assert: verify returned administrators */
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact(DisplayName = "[integration] - when 'filter administrators async' is called with name filter, only matching administrators are returned")]
    public async Task When_FilterAdministratorsAsync_IsCalled_WithNameFilter_Then_OnlyMatchingAdministratorsAreReturned()
    {
        /* arrange: insert administrators with distinct names into postgres */
        var administrators = _fixture.Build<Administrator>()
            .With(entity => entity.Code, () => Guid.NewGuid().ToString("N")[..8])
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .CreateMany(3)
            .ToList();

        administrators[0].Name = "john doe";

        await postgres.Context.Administrators.AddRangeAsync(administrators, TestContext.Current.CancellationToken);
        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new AdministratorRepository(postgres.Context, NullLogger<AdministratorRepository>.Instance);
        var filters = AdministratorFilters.AsBuilder()
            .WithName("john")
            .Build();

        /* act: query administrators by name */
        var result = await repository.FilterAdministratorsAsync(filters, TestContext.Current.CancellationToken);

        /* assert: verify only the matching administrator is returned */
        Assert.Single(result);
        Assert.Equal(administrators[0].Id, result.First().Id);
    }

    [Fact(DisplayName = "[integration] - when 'filter administrators async' is called with code filter, only matching administrators are returned")]
    public async Task When_FilterAdministratorsAsync_IsCalled_WithCodeFilter_Then_OnlyMatchingAdministratorsAreReturned()
    {
        /* arrange: insert administrators with distinct codes into postgres */
        var administrators = _fixture.Build<Administrator>()
            .With(entity => entity.Code, () => Guid.NewGuid().ToString("N")[..8])
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .CreateMany(3)
            .ToList();

        await postgres.Context.Administrators.AddRangeAsync(administrators, TestContext.Current.CancellationToken);
        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new AdministratorRepository(postgres.Context, NullLogger<AdministratorRepository>.Instance);
        var filters = AdministratorFilters.AsBuilder()
            .WithCode(administrators[1].Code)
            .Build();

        /* act: query administrators by code */
        var result = await repository.FilterAdministratorsAsync(filters, TestContext.Current.CancellationToken);

        /* assert: verify only the matching administrator is returned */
        Assert.Single(result);
        Assert.Equal(administrators[1].Id, result.First().Id);
    }

    [Fact(DisplayName = "[integration] - when 'filter administrators async' is called with pagination, only the requested page is returned")]
    public async Task When_FilterAdministratorsAsync_IsCalled_WithPagination_Then_OnlyRequestedPageIsReturned()
    {
        /* arrange: insert administrators into postgres */
        var administrators = _fixture.Build<Administrator>()
            .With(entity => entity.Code, () => Guid.NewGuid().ToString("N")[..8])
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .CreateMany(5);

        await postgres.Context.Administrators.AddRangeAsync(administrators, TestContext.Current.CancellationToken);
        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new AdministratorRepository(postgres.Context, NullLogger<AdministratorRepository>.Instance);
        var filters = AdministratorFilters.WithoutFilters;

        filters.Pagination = new PaginationFilters { PageNumber = 2, PageSize = 2 };

        /* act: query the second page of administrators */
        var result = await repository.FilterAdministratorsAsync(filters, TestContext.Current.CancellationToken);

        /* assert: verify the page size was respected */
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact(DisplayName = "[integration] - when 'filter administrators async' is called with descending sort, administrators are returned in descending order")]
    public async Task When_FilterAdministratorsAsync_IsCalled_WithDescendingSort_Then_AdministratorsAreReturnedInDescendingOrder()
    {
        /* arrange: insert administrators with known names into postgres */
        var administrators = _fixture.Build<Administrator>()
            .With(entity => entity.Code, () => Guid.NewGuid().ToString("N")[..8])
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .CreateMany(3)
            .ToList();

        administrators[0].Name = "alice";
        administrators[1].Name = "charlie";
        administrators[2].Name = "bob";

        await postgres.Context.Administrators.AddRangeAsync(administrators, TestContext.Current.CancellationToken);
        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new AdministratorRepository(postgres.Context, NullLogger<AdministratorRepository>.Instance);
        var filters = AdministratorFilters.WithoutFilters;

        filters.Sort = new SortFilters { Field = nameof(Administrator.Name), Direction = SortDirection.Descending };

        /* act: query administrators sorted by name */
        var result = await repository.FilterAdministratorsAsync(filters, TestContext.Current.CancellationToken);

        /* assert: verify administrators are sorted by name in descending order */
        Assert.Equal(3, result.Count);
        Assert.Equal(["charlie", "bob", "alice"], result.Select(administrator => administrator.Name));
    }

    [Fact(DisplayName = "[integration] - when 'count administrators async' is called without filters, the total of administrators is returned")]
    public async Task When_CountAdministratorsAsync_IsCalled_WithoutFilters_Then_TotalOfAdministratorsIsReturned()
    {
        /* arrange: insert administrators into postgres */
        var administrators = _fixture.Build<Administrator>()
            .With(entity => entity.Code, () => Guid.NewGuid().ToString("N")[..8])
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .CreateMany(4);

        await postgres.Context.Administrators.AddRangeAsync(administrators, TestContext.Current.CancellationToken);
        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new AdministratorRepository(postgres.Context, NullLogger<AdministratorRepository>.Instance);

        /* act: count administrators */
        var result = await repository.CountAdministratorsAsync(AdministratorFilters.WithoutFilters, TestContext.Current.CancellationToken);

        /* assert: verify the total of administrators */
        Assert.Equal(4, result);
    }

    [Fact(DisplayName = "[integration] - when 'count administrators async' is called with name filter, only matching administrators are counted")]
    public async Task When_CountAdministratorsAsync_IsCalled_WithNameFilter_Then_OnlyMatchingAdministratorsAreCounted()
    {
        /* arrange: insert administrators with distinct names into postgres */
        var administrators = _fixture.Build<Administrator>()
            .With(entity => entity.Code, () => Guid.NewGuid().ToString("N")[..8])
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .CreateMany(3)
            .ToList();

        administrators[0].Name = "john doe";
        administrators[1].Name = "johnny smith";

        await postgres.Context.Administrators.AddRangeAsync(administrators, TestContext.Current.CancellationToken);
        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new AdministratorRepository(postgres.Context, NullLogger<AdministratorRepository>.Instance);
        var filters = AdministratorFilters.AsBuilder()
            .WithName("john")
            .Build();

        /* act: count administrators by name */
        var result = await repository.CountAdministratorsAsync(filters, TestContext.Current.CancellationToken);

        /* assert: verify only the matching administrators are counted */
        Assert.Equal(2, result);
    }

    [Fact(DisplayName = "[integration] - when 'insert async' is called, the administrator is persisted")]
    public async Task When_InsertAsync_IsCalled_Then_AdministratorIsPersisted()
    {
        /* arrange: create administrator */
        var repository = new AdministratorRepository(postgres.Context, NullLogger<AdministratorRepository>.Instance);
        var administrator = _fixture.Build<Administrator>()
            .With(entity => entity.Code, () => Guid.NewGuid().ToString("N")[..8])
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .Create();

        /* act: persist administrator */
        var inserted = await repository.InsertAsync(administrator, cancellation: TestContext.Current.CancellationToken);

        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        /* assert: verify administrator was persisted */
        var persisted = await postgres.Context.Administrators
            .AsNoTracking()
            .FirstOrDefaultAsync(persisted => persisted.Id == inserted.Id, TestContext.Current.CancellationToken);

        Assert.NotNull(persisted);
        Assert.False(string.IsNullOrWhiteSpace(persisted.Id));

        Assert.Equal(administrator.Name, persisted.Name);
        Assert.Equal(administrator.Code, persisted.Code);
    }

    [Fact(DisplayName = "[integration] - when 'insert many async' is called, all administrators are persisted")]
    public async Task When_InsertManyAsync_IsCalled_Then_AllAdministratorsArePersisted()
    {
        /* arrange: create administrators */
        var repository = new AdministratorRepository(postgres.Context, NullLogger<AdministratorRepository>.Instance);
        var administrators = _fixture.Build<Administrator>()
            .With(entity => entity.Code, () => Guid.NewGuid().ToString("N")[..8])
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .CreateMany(3)
            .ToList();

        /* act: persist administrators */
        await repository.InsertManyAsync(administrators, cancellation: TestContext.Current.CancellationToken);
        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        /* assert: verify all administrators were persisted */
        var persisted = await postgres.Context.Administrators
            .AsNoTracking()
            .ToListAsync(TestContext.Current.CancellationToken);

        Assert.Equal(3, persisted.Count);
        Assert.All(administrators, administrator => Assert.Contains(persisted, entity => entity.Id == administrator.Id));
    }

    [Fact(DisplayName = "[integration] - when 'update async' is called, the administrator is updated")]
    public async Task When_UpdateAsync_IsCalled_Then_AdministratorIsUpdated()
    {
        /* arrange: insert administrator into postgres */
        var administrator = _fixture.Build<Administrator>()
            .With(entity => entity.Code, () => Guid.NewGuid().ToString("N")[..8])
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .Create();

        await postgres.Context.Administrators.AddAsync(administrator, TestContext.Current.CancellationToken);
        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        postgres.Context.ChangeTracker.Clear();

        var repository = new AdministratorRepository(postgres.Context, NullLogger<AdministratorRepository>.Instance);

        /* act: change the administrator's name and persist the update */
        administrator.Name = "updated administrator name";

        await repository.UpdateAsync(administrator, TestContext.Current.CancellationToken);
        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        /* assert: verify the change was persisted */
        var persisted = await postgres.Context.Administrators
            .AsNoTracking()
            .FirstOrDefaultAsync(persisted => persisted.Id == administrator.Id, TestContext.Current.CancellationToken);

        Assert.NotNull(persisted);
        Assert.Equal("updated administrator name", persisted.Name);
    }

    [Fact(DisplayName = "[integration] - when 'delete async' is called with soft behavior, the administrator is marked as deleted")]
    public async Task When_DeleteAsync_IsCalled_WithSoftBehavior_Then_AdministratorIsMarkedAsDeleted()
    {
        /* arrange: insert administrator into postgres */
        var administrator = _fixture.Build<Administrator>()
            .With(entity => entity.Code, () => Guid.NewGuid().ToString("N")[..8])
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .Create();

        await postgres.Context.Administrators.AddAsync(administrator, TestContext.Current.CancellationToken);
        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new AdministratorRepository(postgres.Context, NullLogger<AdministratorRepository>.Instance);

        /* act: soft delete the administrator */
        var result = await repository.DeleteAsync(administrator, DeletionBehavior.Soft, TestContext.Current.CancellationToken);

        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        /* assert: verify the administrator remains in the table but is marked as deleted */
        var persisted = await postgres.Context.Administrators
            .AsNoTracking()
            .FirstOrDefaultAsync(persisted => persisted.Id == administrator.Id, TestContext.Current.CancellationToken);

        Assert.True(result);

        Assert.NotNull(persisted);
        Assert.True(persisted.IsDeleted);
    }

    [Fact(DisplayName = "[integration] - when 'delete async' is called with hard behavior, the administrator is removed from the table")]
    public async Task When_DeleteAsync_IsCalled_WithHardBehavior_Then_AdministratorIsRemoved()
    {
        /* arrange: insert administrator into postgres */
        var administrator = _fixture.Build<Administrator>()
            .With(entity => entity.Code, () => Guid.NewGuid().ToString("N")[..8])
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .Create();

        await postgres.Context.Administrators.AddAsync(administrator, TestContext.Current.CancellationToken);
        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new AdministratorRepository(postgres.Context, NullLogger<AdministratorRepository>.Instance);

        /* act: hard delete the administrator */
        var result = await repository.DeleteAsync(administrator, DeletionBehavior.Hard, TestContext.Current.CancellationToken);

        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        /* assert: verify the administrator no longer exists in the table */
        var persisted = await postgres.Context.Administrators
            .AsNoTracking()
            .FirstOrDefaultAsync(persisted => persisted.Id == administrator.Id, TestContext.Current.CancellationToken);

        Assert.True(result);
        Assert.Null(persisted);
    }

    public async ValueTask DisposeAsync() => await ValueTask.FromResult(ValueTask.CompletedTask);
    public async ValueTask InitializeAsync() => await postgres.CleanDatabaseAsync();
}
