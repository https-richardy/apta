namespace Apta.WebUI.Services;

public sealed class AdministratorsService(IQueryClient queryClient, IAdministratorsClient administratorsClient)
{
    public IQuery<AdministratorFiltersParameters, Pagination<AdministratorScheme>> Query { get; private set; } =
        queryClient.CreateQuery(new QueryOptions<AdministratorFiltersParameters, Pagination<AdministratorScheme>>
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

    public IMutation<AdministratorCreationScheme, AdministratorScheme> CreateMutation { get; private set; } =
        queryClient.CreateMutation(new MutationOptions<AdministratorCreationScheme, AdministratorScheme>
        {
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

    public IMutation<UpdateAdministratorScheme, AdministratorScheme> UpdateMutation { get; private set; } =
        queryClient.CreateMutation(new MutationOptions<UpdateAdministratorScheme, AdministratorScheme>
        {
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

    public IMutation<AdministratorDeletionScheme, Result> DeletionMutation { get; private set; } =
        queryClient.CreateMutation(new MutationOptions<AdministratorDeletionScheme, Result>
        {
            Mutator = async (scheme, cancellation) =>
            {
                var result = await administratorsClient.DeleteAdministratorAsync(scheme, cancellation);
                if (result.IsFailure)
                {
                    throw new InvalidOperationException("não foi possível excluir a administradora");
                }

                return result;
            }
        });
}
