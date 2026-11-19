namespace Apta.Domain.Services;

public interface ICustomerParser
{
    public Task<Result<IReadOnlyCollection<Customer>>> ParseCustomersAsync(
        Stream stream,
        CancellationToken cancellation = default
    );
}
