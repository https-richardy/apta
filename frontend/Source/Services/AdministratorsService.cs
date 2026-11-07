namespace Apta.WebUI.Services;

public sealed class AdministratorsService(IQueryClient queryClient, IAdministratorsClient administratorsClient)
{
    public IQuery<AdministratorFiltersParameters, Pagination<AdministratorScheme>> Query { get; private set; } =
        queryClient.CreateQuery(new QueryOptions<AdministratorFiltersParameters, Pagination<AdministratorScheme>>
        {
            KeyFactory = filters => QueryKey.From("administrators", filters.Page, filters.PageSize),
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

    public IMutation<AdministratorCreationScheme, AdministratorScheme> CreateMutation { get; private set; } =
        queryClient.CreateMutation(new MutationOptions<AdministratorCreationScheme, AdministratorScheme>
        {
            OnSuccess = (_, _) => queryClient.Invalidate(key => key.ToString().StartsWith("administrators")),
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
}
