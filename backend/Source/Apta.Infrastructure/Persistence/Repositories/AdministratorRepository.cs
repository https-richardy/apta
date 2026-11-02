namespace Apta.Infrastructure.Persistence.Repositories;

public sealed class AdministratorRepository(DatabaseContext database, ILogger<AdministratorRepository> logger) :
    AggregateRepository<Administrator>(database, logger),
    IAdministratorRepository
{
    private readonly DbContext _database = database;

    public async Task<IReadOnlyCollection<Administrator>> FilterAdministratorsAsync(AdministratorFilters filters, CancellationToken cancellation = default)
    {
        var pipeline = _database.Set<Administrator>()
            .AsNoTracking()
            .FilterAdministrators(filters)
            .Sort(filters.Sort)
            .Paginate(filters.Pagination);

        return await pipeline.ToListAsync(cancellation);
    }

    public async Task<System.Numerics.BigInteger> CountAdministratorsAsync(AdministratorFilters filters, CancellationToken cancellation = default)
    {
        var pipeline = _database.Set<Administrator>()
            .AsNoTracking()
            .FilterAdministrators(filters);

        return await pipeline.CountAsync(cancellation);
    }
}
