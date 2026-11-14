namespace Apta.WebUI.Services;

public sealed class CustomersService
{
    public IQuery<CustomerFiltersParameters, Pagination<CustomerScheme>> Query { get; private set; }

    public CustomersService(IQueryClient queryClient, ICustomersClient customersClient)
    {
        Query = queryClient.CreateQuery(new QueryOptions<CustomerFiltersParameters, Pagination<CustomerScheme>>
        {
            KeyFactory = filters => QueryKey.From("customers", filters),
            Fetcher = async (filters, cancellation) =>
            {
                var result = await customersClient.GetCustomersAsync(filters, cancellation);
                if (result.IsFailure || result.Data is null)
                {
                    throw new InvalidOperationException("não foi possível carregar os clientes");
                }

                return result.Data;
            }
        });
    }
}
