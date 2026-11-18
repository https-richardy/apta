namespace Apta.WebUI.Services;

public sealed class CustomersService
{
    public IQuery<CustomerFiltersParameters, Pagination<CustomerScheme>> Query { get; private set; }
    public IMutation<CustomerCreationScheme, CustomerScheme> CreateMutation { get; private set; }
    public IMutation<UpdateCustomerScheme, CustomerScheme> UpdateMutation { get; private set; }
    public IMutation<CustomerDeletionScheme, Result> DeletionMutation { get; private set; }

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

        CreateMutation = queryClient.CreateMutation(new MutationOptions<CustomerCreationScheme, CustomerScheme>
        {
            OnSuccess = (_, _) => Query.Refetch(),
            Mutator = async (scheme, cancellation) =>
            {
                var result = await customersClient.CreateCustomerAsync(scheme, cancellation);
                if (result.IsFailure || result.Data is null)
                {
                    throw new InvalidOperationException("não foi possível criar o cliente");
                }

                return result.Data;
            }
        });

        UpdateMutation = queryClient.CreateMutation(new MutationOptions<UpdateCustomerScheme, CustomerScheme>
        {
            OnSuccess = (_, _) => Query.Refetch(),
            Mutator = async (scheme, cancellation) =>
            {
                var result = await customersClient.UpdateCustomerAsync(scheme, cancellation);
                if (result.IsFailure || result.Data is null)
                {
                    throw new InvalidOperationException("não foi possível atualizar o cliente");
                }

                return result.Data;
            }
        });

        DeletionMutation = queryClient.CreateMutation(new MutationOptions<CustomerDeletionScheme, Result>
        {
            OnSuccess = (_, _) => Query.Refetch(),
            Mutator = async (scheme, cancellation) =>
            {
                var result = await customersClient.DeleteCustomerAsync(scheme, cancellation);
                if (result.IsFailure)
                {
                    throw new InvalidOperationException("não foi possível excluir o cliente");
                }

                return result;
            }
        });
    }
}
