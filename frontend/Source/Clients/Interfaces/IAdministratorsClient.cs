namespace Apta.WebUI.Clients.Interfaces;

public interface IAdministratorsClient
{
    public Task<Result<AdministratorScheme>> CreateAdministratorAsync(
        AdministratorCreationScheme parameters,
        CancellationToken cancellation = default
    );

    public Task<Result<AdministratorScheme>> UpdateAdministratorAsync(
        UpdateAdministratorScheme parameters,
        CancellationToken cancellation = default
    );

    public Task<Result> DeleteAdministratorAsync(
        AdministratorDeletionScheme parameters,
        CancellationToken cancellation = default
    );

    public Task<Result<Pagination<AdministratorScheme>>> GetAdministratorsAsync(
        AdministratorFiltersParameters parameters,
        CancellationToken cancellation = default
    );
}
