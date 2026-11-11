namespace Apta.Application.Handlers.Administrators;

public sealed class AdministratorDeletionHandler(IAdministratorRepository repository, IUnitOfWork unitOfWork) :
    IDispatchHandler<AdministratorDeletionScheme, Result>
{
    public async Task<Result> HandleAsync(AdministratorDeletionScheme parameters, CancellationToken cancellation = default)
    {
        var filters = AdministratorFilters.AsBuilder()
            .WithIdentifier(parameters.Id)
            .Build();

        var administrators = await repository.FilterAdministratorsAsync(filters, cancellation);
        var administrator = administrators.FirstOrDefault();

        if (administrator is null)
        {
            return Result.Failure(AdministratorErrors.DoesNotExist);
        }

        await unitOfWork.TransactionAsync(cancellation, async cancellationToken =>
        {
            await repository.DeleteAsync(administrator, cancellation: cancellationToken);
        });

        return Result.Success();
    }
}
