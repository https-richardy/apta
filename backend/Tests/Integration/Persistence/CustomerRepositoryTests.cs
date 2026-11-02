namespace Apta.Tests.Integration.Persistence;

[Collection(nameof(DisableParallelizationCollection))]
public sealed class CustomerRepositoryTests(PostgresDatabase postgres) :
    IClassFixture<PostgresDatabase>,
    IAsyncLifetime
{
    private readonly Fixture _fixture = new();

    [Fact(DisplayName = "[integration] - when 'filter customers async' is called without filters, all customers are returned")]
    public async Task When_FilterCustomersAsync_IsCalled_WithoutFilters_Then_AllCustomersAreReturned()
    {
        /* arrange: insert customers into postgres */
        var customers = _fixture.Build<Customer>()
            .With(entity => entity.Phone, "+552128756433")
            .With(entity => entity.Document, () => $"{Random.Shared.NextInt64(10000000000000, 99999999999999)}")
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .CreateMany(2)
            .ToList();

        await postgres.Context.Customers.AddRangeAsync(customers, TestContext.Current.CancellationToken);
        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new CustomerRepository(postgres.Context, NullLogger<CustomerRepository>.Instance);

        /* act: query customers */
        var result = await repository.FilterCustomersAsync(CustomerFilters.WithoutFilters, TestContext.Current.CancellationToken);

        /* assert: verify returned customers */
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact(DisplayName = "[integration] - when 'filter customers async' is called with name filter, only matching customers are returned")]
    public async Task When_FilterCustomersAsync_IsCalled_WithNameFilter_Then_OnlyMatchingCustomersAreReturned()
    {
        /* arrange: insert customers with distinct names into postgres */
        var customers = _fixture.Build<Customer>()
            .With(entity => entity.Phone, () => $"+552128756433")
            .With(entity => entity.Document, () => $"{Random.Shared.NextInt64(10000000000000, 99999999999999)}")
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .CreateMany(3)
            .ToList();

        customers[0].Name = "maria silva";

        await postgres.Context.Customers.AddRangeAsync(customers, TestContext.Current.CancellationToken);
        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new CustomerRepository(postgres.Context, NullLogger<CustomerRepository>.Instance);
        var filters = CustomerFilters.AsBuilder()
            .WithName("maria")
            .Build();

        /* act: query customers by name */
        var result = await repository.FilterCustomersAsync(filters, TestContext.Current.CancellationToken);

        /* assert: verify only the matching customer is returned */
        Assert.Single(result);
        Assert.Equal(customers[0].Id, result.First().Id);
    }

    [Fact(DisplayName = "[integration] - when 'filter customers async' is called with phone filter, only matching customers are returned")]
    public async Task When_FilterCustomersAsync_IsCalled_WithPhoneFilter_Then_OnlyMatchingCustomersAreReturned()
    {
        /* arrange: insert customers with distinct phones into postgres */
        var customers = _fixture.Build<Customer>()
            .With(entity => entity.Phone, () => $"+552128756433")
            .With(entity => entity.Document, () => $"{Random.Shared.NextInt64(10000000000000, 99999999999999)}")
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .CreateMany(3)
            .ToList();

        customers[1].Phone = "+5531998472615";

        await postgres.Context.Customers.AddRangeAsync(customers, TestContext.Current.CancellationToken);
        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new CustomerRepository(postgres.Context, NullLogger<CustomerRepository>.Instance);
        var filters = CustomerFilters.AsBuilder()
            .WithPhone(customers[1].Phone)
            .Build();

        /* act: query customers by phone */
        var result = await repository.FilterCustomersAsync(filters, TestContext.Current.CancellationToken);

        /* assert: verify only the matching customer is returned */
        Assert.Single(result);
        Assert.Equal(customers[1].Id, result.First().Id);
    }

    [Fact(DisplayName = "[integration] - when 'filter customers async' is called with formatted document filter, the customer is found by its digits")]
    public async Task When_FilterCustomersAsync_IsCalled_WithFormattedDocumentFilter_Then_CustomerIsFoundByItsDigits()
    {
        /* arrange: insert customers into postgres, one of them with a known document */
        var customers = _fixture.Build<Customer>()
            .With(entity => entity.Phone, () => $"+552128756433")
            .With(entity => entity.Document, () => $"{Random.Shared.NextInt64(10000000000000, 99999999999999)}")
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .CreateMany(3)
            .ToList();

        customers[2].Document = "38619247573";

        await postgres.Context.Customers.AddRangeAsync(customers, TestContext.Current.CancellationToken);
        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new CustomerRepository(postgres.Context, NullLogger<CustomerRepository>.Instance);
        var filters = CustomerFilters.AsBuilder()
            .WithDocument("386.192.475-73")
            .Build();

        /* act: query customers by a formatted document */
        var result = await repository.FilterCustomersAsync(filters, TestContext.Current.CancellationToken);

        /* assert: verify only the matching customer is returned */
        Assert.Single(result);
        Assert.Equal(customers[2].Id, result.First().Id);
    }

    [Fact(DisplayName = "[integration] - when 'filter customers async' is called with pagination, only the requested page is returned")]
    public async Task When_FilterCustomersAsync_IsCalled_WithPagination_Then_OnlyRequestedPageIsReturned()
    {
        /* arrange: insert customers into postgres */
        var customers = _fixture.Build<Customer>()
            .With(entity => entity.Phone, () => $"+552128756433")
            .With(entity => entity.Document, () => $"{Random.Shared.NextInt64(10000000000000, 99999999999999)}")
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .CreateMany(5)
            .ToList();

        await postgres.Context.Customers.AddRangeAsync(customers, TestContext.Current.CancellationToken);
        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new CustomerRepository(postgres.Context, NullLogger<CustomerRepository>.Instance);
        var filters = CustomerFilters.WithoutFilters;

        filters.Pagination = PaginationFilters.From(pageSize: 2, pageNumber: 3);

        /* act: query the last page of customers */
        var result = await repository.FilterCustomersAsync(filters, TestContext.Current.CancellationToken);

        /* assert: verify only the remaining customer is returned */
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact(DisplayName = "[integration] - when 'filter customers async' is called with ascending sort, customers are returned in ascending order")]
    public async Task When_FilterCustomersAsync_IsCalled_WithAscendingSort_Then_CustomersAreReturnedInAscendingOrder()
    {
        /* arrange: insert customers with known names into postgres */
        var customers = _fixture.Build<Customer>()
            .With(entity => entity.Phone, "+552128756433")
            .With(entity => entity.Document, () => $"{Random.Shared.NextInt64(10000000000000, 99999999999999)}")
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .CreateMany(3)
            .ToList();

        customers[0].Name = "carla";
        customers[1].Name = "ana";
        customers[2].Name = "bruno";

        await postgres.Context.Customers.AddRangeAsync(customers, TestContext.Current.CancellationToken);
        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new CustomerRepository(postgres.Context, NullLogger<CustomerRepository>.Instance);
        var filters = CustomerFilters.WithoutFilters;

        filters.Sort = new SortFilters { Field = nameof(Customer.Name), Direction = SortDirection.Ascending };

        /* act: query customers sorted by name */
        var result = await repository.FilterCustomersAsync(filters, TestContext.Current.CancellationToken);

        /* assert: verify customers are sorted by name in ascending order */
        Assert.Equal(3, result.Count);
        Assert.Equal(["ana", "bruno", "carla"], result.Select(customer => customer.Name));
    }

    [Fact(DisplayName = "[integration] - when 'count customers async' is called without filters, the total of customers is returned")]
    public async Task When_CountCustomersAsync_IsCalled_WithoutFilters_Then_TotalOfCustomersIsReturned()
    {
        /* arrange: insert customers into postgres */
        var customers = _fixture.Build<Customer>()
            .With(entity => entity.Phone, "+552128756433")
            .With(entity => entity.Document, () => $"{Random.Shared.NextInt64(10000000000000, 99999999999999)}")
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .CreateMany(4);

        await postgres.Context.Customers.AddRangeAsync(customers, TestContext.Current.CancellationToken);
        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new CustomerRepository(postgres.Context, NullLogger<CustomerRepository>.Instance);

        /* act: count customers */
        var result = await repository.CountCustomersAsync(CustomerFilters.WithoutFilters, TestContext.Current.CancellationToken);

        /* assert: verify the total of customers */
        Assert.Equal(4, result);
    }

    [Fact(DisplayName = "[integration] - when 'count customers async' is called with name filter, only matching customers are counted")]
    public async Task When_CountCustomersAsync_IsCalled_WithNameFilter_Then_OnlyMatchingCustomersAreCounted()
    {
        /* arrange: insert customers with distinct names into postgres */
        var customers = _fixture.Build<Customer>()
            .With(entity => entity.Phone, "+552128756433")
            .With(entity => entity.Document, () => $"{Random.Shared.NextInt64(10000000000000, 99999999999999)}")
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .CreateMany(3)
            .ToList();

        customers[0].Name = "maria silva";
        customers[1].Name = "maria souza";

        await postgres.Context.Customers.AddRangeAsync(customers, TestContext.Current.CancellationToken);
        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new CustomerRepository(postgres.Context, NullLogger<CustomerRepository>.Instance);
        var filters = CustomerFilters.AsBuilder()
            .WithName("maria")
            .Build();

        /* act: count customers by name */
        var result = await repository.CountCustomersAsync(filters, TestContext.Current.CancellationToken);

        /* assert: verify only the matching customers are counted */
        Assert.Equal(2, result);
    }

    [Fact(DisplayName = "[integration] - when 'insert async' is called, the customer is persisted")]
    public async Task When_InsertAsync_IsCalled_Then_CustomerIsPersisted()
    {
        /* arrange: create customer */
        var repository = new CustomerRepository(postgres.Context, NullLogger<CustomerRepository>.Instance);
        var customer = _fixture.Build<Customer>()
            .With(entity => entity.Phone, "+552128756433")
            .With(entity => entity.Document, "11485127726")
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .Create();

        /* act: persist customer */
        var inserted = await repository.InsertAsync(customer, cancellation: TestContext.Current.CancellationToken);

        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        /* assert: verify customer was persisted */
        var persisted = await postgres.Context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(persisted => persisted.Id == inserted.Id, TestContext.Current.CancellationToken);

        Assert.NotNull(persisted);
        Assert.False(string.IsNullOrWhiteSpace(persisted.Id));

        Assert.Equal(customer.Name, persisted.Name);
        Assert.Equal(customer.Phone, persisted.Phone);
        Assert.Equal(customer.Document, persisted.Document);
    }

    [Fact(DisplayName = "[integration] - when 'insert many async' is called, all customers are persisted")]
    public async Task When_InsertManyAsync_IsCalled_Then_AllCustomersArePersisted()
    {
        /* arrange: create customers */
        var repository = new CustomerRepository(postgres.Context, NullLogger<CustomerRepository>.Instance);
        var customers = _fixture.Build<Customer>()
            .With(entity => entity.Phone, "+552128756433")
            .With(entity => entity.Document, () => $"{Random.Shared.NextInt64(10000000000000, 99999999999999)}")
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .CreateMany(3)
            .ToList();

        /* act: persist customers */
        await repository.InsertManyAsync(customers, cancellation: TestContext.Current.CancellationToken);
        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        /* assert: verify all customers were persisted */
        var persisted = await postgres.Context.Customers
            .AsNoTracking()
            .ToListAsync(TestContext.Current.CancellationToken);

        Assert.Equal(3, persisted.Count);
        Assert.All(customers, customer => Assert.Contains(persisted, entity => entity.Id == customer.Id));
    }

    [Fact(DisplayName = "[integration] - when 'update async' is called, the customer is updated")]
    public async Task When_UpdateAsync_IsCalled_Then_CustomerIsUpdated()
    {
        /* arrange: insert customer into postgres */
        var customer = _fixture.Build<Customer>()
            .With(entity => entity.Phone, "+552128756433")
            .With(entity => entity.Document, "11485127726")
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .Create();

        await postgres.Context.Customers.AddAsync(customer, TestContext.Current.CancellationToken);
        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        postgres.Context.ChangeTracker.Clear();

        var repository = new CustomerRepository(postgres.Context, NullLogger<CustomerRepository>.Instance);

        /* act: change the customer's name and phone and persist the update */
        customer.Name = "updated customer name";
        customer.Phone = "+5531998472615";

        await repository.UpdateAsync(customer, TestContext.Current.CancellationToken);
        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        /* assert: verify the changes were persisted */
        var persisted = await postgres.Context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(persisted => persisted.Id == customer.Id, TestContext.Current.CancellationToken);

        Assert.NotNull(persisted);

        Assert.Equal("updated customer name", persisted.Name);
        Assert.Equal("+5531998472615", persisted.Phone);
    }

    [Fact(DisplayName = "[integration] - when 'delete async' is called with soft behavior, the customer is marked as deleted")]
    public async Task When_DeleteAsync_IsCalled_WithSoftBehavior_Then_CustomerIsMarkedAsDeleted()
    {
        /* arrange: insert customer into postgres */
        var customer = _fixture.Build<Customer>()
            .With(entity => entity.Phone, "+552128756433")
            .With(entity => entity.Document, "11485127726")
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .Create();

        await postgres.Context.Customers.AddAsync(customer, TestContext.Current.CancellationToken);
        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new CustomerRepository(postgres.Context, NullLogger<CustomerRepository>.Instance);

        /* act: soft delete the customer */
        var result = await repository.DeleteAsync(customer, DeletionBehavior.Soft, TestContext.Current.CancellationToken);

        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        /* assert: verify the customer remains in the table but is marked as deleted */
        var persisted = await postgres.Context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(persisted => persisted.Id == customer.Id, TestContext.Current.CancellationToken);

        Assert.True(result);

        Assert.NotNull(persisted);
        Assert.True(persisted.IsDeleted);
    }

    [Fact(DisplayName = "[integration] - when 'delete async' is called with hard behavior, the customer is removed from the table")]
    public async Task When_DeleteAsync_IsCalled_WithHardBehavior_Then_CustomerIsRemoved()
    {
        /* arrange: insert customer into postgres */
        var customer = _fixture.Build<Customer>()
            .With(entity => entity.Phone, "+552128756433")
            .With(entity => entity.Document, "11485127726")
            .With(entity => entity.CreatedAt, DateTime.UtcNow)
            .With(entity => entity.UpdatedAt, DateTime.UtcNow)
            .With(entity => entity.IsDeleted, false)
            .Create();

        await postgres.Context.Customers.AddAsync(customer, TestContext.Current.CancellationToken);
        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var repository = new CustomerRepository(postgres.Context, NullLogger<CustomerRepository>.Instance);

        /* act: hard delete the customer */
        var result = await repository.DeleteAsync(customer, DeletionBehavior.Hard, TestContext.Current.CancellationToken);

        await postgres.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        /* assert: verify the customer no longer exists in the table */
        var persisted = await postgres.Context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(persisted => persisted.Id == customer.Id, TestContext.Current.CancellationToken);

        Assert.True(result);
        Assert.Null(persisted);
    }

    public async ValueTask DisposeAsync() => await ValueTask.FromResult(ValueTask.CompletedTask);
    public async ValueTask InitializeAsync() => await postgres.CleanDatabaseAsync();
}
