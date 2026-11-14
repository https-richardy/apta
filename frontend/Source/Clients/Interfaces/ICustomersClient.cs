namespace Apta.WebUI.Clients.Interfaces;

public interface ICustomersClient
{
    public Task<Result<CustomerScheme>> CreateCustomerAsync(
        CustomerCreationScheme parameters,
        CancellationToken cancellation = default
    );

    public Task<Result<Pagination<CustomerScheme>>> GetCustomersAsync(
        CustomerFiltersParameters parameters,
        CancellationToken cancellation = default
    );
}
