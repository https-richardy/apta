namespace Apta.Domain.Repositories;

public interface ICustomerRepository : IAggregateCollection<Customer>
{
    public Task<IReadOnlyCollection<Customer>> FilterCustomersAsync(
        CustomerFilters filters,
        CancellationToken cancellation = default
    );

    public Task<System.Numerics.BigInteger> CountCustomersAsync(
        CustomerFilters filters,
        CancellationToken cancellation = default
    );
}
