namespace Apta.WebUI.Clients.Interfaces;

public interface ICustomersClient
{
    public Task<Result<CustomerScheme>> CreateCustomerAsync(
        CustomerCreationScheme parameters,
        CancellationToken cancellation = default
    );

    public Task<Result<CustomerScheme>> UpdateCustomerAsync(
        UpdateCustomerScheme parameters,
        CancellationToken cancellation = default
    );

    public Task<Result> DeleteCustomerAsync(
        CustomerDeletionScheme parameters,
        CancellationToken cancellation = default
    );

    public Task<Result<Pagination<CustomerScheme>>> GetCustomersAsync(
        CustomerFiltersParameters parameters,
        CancellationToken cancellation = default
    );

    public Task<Result> ImportCustomersAsync(
        CustomersImportScheme parameters,
        CancellationToken cancellation = default
    );
}
