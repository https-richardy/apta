namespace Apta.Application.Handlers.Administrators;

public sealed class AdministratorCreationHandler(IAdministratorRepository repository, IUnitOfWork unitOfWork, IMapper mapper) :
    IDispatchHandler<AdministratorCreationScheme, Result<AdministratorScheme>>
{
    public async Task<Result<AdministratorScheme>> HandleAsync(AdministratorCreationScheme parameters, CancellationToken cancellation = default)
    {
        var filters = AdministratorFilters.AsBuilder()
            .WithCode(parameters.Code)
            .Build();

        var administrators = await repository.FilterAdministratorsAsync(filters, cancellation);
        if (administrators.Any(administrator => administrator.Code == parameters.Code))
        {
            return Result<AdministratorScheme>.Failure(AdministratorErrors.AlreadyExists);
        }

        var administrator = mapper.Map<Administrator>(parameters);
        var details = await unitOfWork.TransactionAsync(cancellation, async cancellationToken =>
        {
            var persistedAdministrator = await repository.InsertAsync(administrator, cancellation: cancellationToken);

            return mapper.Map<AdministratorScheme>(persistedAdministrator);
        });

        return Result<AdministratorScheme>.Success(details);
    }
}
