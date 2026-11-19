namespace Apta.WebUI.Services;

public sealed class CustomersService(IQueryClient queryClient, ICustomersClient customersClient, INotificationService notification)
{
    public IQuery<CustomerFiltersParameters, Pagination<CustomerScheme>> Query { get; private set; } =
        queryClient.CreateQuery(new QueryOptions<CustomerFiltersParameters, Pagination<CustomerScheme>>
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

    public IMutation<CustomerCreationScheme, CustomerScheme> CreateMutation { get; private set; } =
        queryClient.CreateMutation(new MutationOptions<CustomerCreationScheme, CustomerScheme>
        {
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

    public IMutation<UpdateCustomerScheme, CustomerScheme> UpdateMutation { get; private set; } =
        queryClient.CreateMutation(new MutationOptions<UpdateCustomerScheme, CustomerScheme>
        {
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

    public IMutation<CustomerDeletionScheme, Result> DeletionMutation { get; private set; } =
        queryClient.CreateMutation(new MutationOptions<CustomerDeletionScheme, Result>
        {
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

    public IMutation<CustomersImportScheme, Result> ImportMutation { get; private set; } =
        queryClient.CreateMutation(new MutationOptions<CustomersImportScheme, Result>
        {
            OnSuccess = (_, _) => notification.ShowSuccessToastAsync("Importação iniciada", "O arquivo foi recebido e os clientes estão sendo importados em segundo plano."),
            Mutator = async (scheme, cancellation) =>
            {
                var result = await customersClient.ImportCustomersAsync(scheme, cancellation);
                if (result.IsFailure)
                {
                    throw new InvalidOperationException("não foi possível importar os clientes");
                }

                return result;
            }
        });
}
