namespace Apta.Infrastructure.Persistence.Repositories;

public sealed class CustomerRepository(DatabaseContext database, ILogger<CustomerRepository> logger) :
    AggregateRepository<Customer>(database, logger),
    ICustomerRepository
{
    private readonly DbContext _database = database;

    public async Task<IReadOnlyCollection<Customer>> FilterCustomersAsync(CustomerFilters filters, CancellationToken cancellation = default)
    {
        var pipeline = _database.Set<Customer>()
            .AsNoTracking()
            .FilterCustomers(filters)
            .Sort(filters.Sort)
            .Paginate(filters.Pagination);

        return await pipeline.ToListAsync(cancellation);
    }

    public async Task<System.Numerics.BigInteger> CountCustomersAsync(CustomerFilters filters, CancellationToken cancellation = default)
    {
        var pipeline = _database.Set<Customer>()
            .AsNoTracking()
            .FilterCustomers(filters);

        return await pipeline.CountAsync(cancellation);
    }
}
