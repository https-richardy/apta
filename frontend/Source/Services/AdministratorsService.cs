namespace Apta.WebUI.Services;

public sealed class AdministratorsService
{
    public IQuery<AdministratorFiltersParameters, Pagination<AdministratorScheme>> Query { get; private set; }
    public IMutation<AdministratorCreationScheme, AdministratorScheme> CreateMutation { get; private set; }
    public IMutation<UpdateAdministratorScheme, AdministratorScheme> UpdateMutation { get; private set; }

    public AdministratorsService(IQueryClient queryClient, IAdministratorsClient administratorsClient)
    {
        Query = queryClient.CreateQuery(new QueryOptions<AdministratorFiltersParameters, Pagination<AdministratorScheme>>
        {
            KeyFactory = filters => QueryKey.From("administrators", filters),
            Fetcher = async (filters, cancellation) =>
            {
                var result = await administratorsClient.GetAdministratorsAsync(filters, cancellation);
                if (result.IsFailure || result.Data is null)
                {
                    throw new InvalidOperationException("não foi possível carregar as administradoras");
                }

                return result.Data;
            }
        });

        CreateMutation = queryClient.CreateMutation(new MutationOptions<AdministratorCreationScheme, AdministratorScheme>
        {
            OnSuccess = (_, _) => Query.Refetch(),
            Mutator = async (scheme, cancellation) =>
            {
                var result = await administratorsClient.CreateAdministratorAsync(scheme, cancellation);
                if (result.IsFailure || result.Data is null)
                {
                    throw new InvalidOperationException("não foi possível criar a administradora");
                }

                return result.Data;
            }
        });

        UpdateMutation = queryClient.CreateMutation(new MutationOptions<UpdateAdministratorScheme, AdministratorScheme>
        {
            OnSuccess = (_, _) => Query.Refetch(),
            Mutator = async (scheme, cancellation) =>
            {
                var result = await administratorsClient.UpdateAdministratorAsync(scheme, cancellation);
                if (result.IsFailure || result.Data is null)
                {
                    throw new InvalidOperationException("não foi possível atualizar a administradora");
                }

                return result.Data;
            }
        });
    }
}
