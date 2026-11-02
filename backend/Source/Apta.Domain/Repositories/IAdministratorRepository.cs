namespace Apta.Domain.Repositories;

public interface IAdministratorRepository : IAggregateCollection<Administrator>
{
    public Task<IReadOnlyCollection<Administrator>> FilterAdministratorsAsync(
        AdministratorFilters filters,
        CancellationToken cancellation = default
    );

    public Task<System.Numerics.BigInteger> CountAdministratorsAsync(
        AdministratorFilters filters,
        CancellationToken cancellation = default
    );
}
