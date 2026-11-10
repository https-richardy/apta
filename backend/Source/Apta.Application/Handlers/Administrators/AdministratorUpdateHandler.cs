namespace Apta.Application.Handlers.Administrators;

public sealed class AdministratorUpdateHandler(IAdministratorRepository repository, IUnitOfWork unitOfWork, IMapper mapper) :
    IDispatchHandler<UpdateAdministratorScheme, Result<AdministratorScheme>>
{
    public async Task<Result<AdministratorScheme>> HandleAsync(UpdateAdministratorScheme parameters, CancellationToken cancellation = default)
    {
        var filters = AdministratorFilters.AsBuilder()
            .WithIdentifier(parameters.Id)
            .Build();

        var administrators = await repository.FilterAdministratorsAsync(filters, cancellation);
        var administrator = administrators.FirstOrDefault();

        if (administrator is null)
        {
            return Result<AdministratorScheme>.Failure(AdministratorErrors.DoesNotExist);
        }

        var duplicateFilters = AdministratorFilters.AsBuilder()
            .WithCode(parameters.Code)
            .Build();

        var administratorsSharingCode = await repository.FilterAdministratorsAsync(duplicateFilters, cancellation);
        if (administratorsSharingCode.Any(other => other.Code == parameters.Code && other.Id != administrator.Id))
        {
            return Result<AdministratorScheme>.Failure(AdministratorErrors.AlreadyExists);
        }

        mapper.Map(parameters, administrator);

        var details = await unitOfWork.TransactionAsync(cancellation, async cancellationToken =>
        {
            var persistedAdministrator = await repository.UpdateAsync(administrator, cancellationToken);

            return mapper.Map<AdministratorScheme>(persistedAdministrator);
        });

        return Result<AdministratorScheme>.Success(details);
    }
}
