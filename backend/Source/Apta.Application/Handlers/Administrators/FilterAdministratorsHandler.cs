namespace Apta.Application.Handlers.Administrators;

public sealed class FilterAdministratorsHandler(IAdministratorRepository repository, IMapper mapper) :
    IDispatchHandler<AdministratorFiltersParameters, Result<Pagination<AdministratorScheme>>>
{
    public async Task<Result<Pagination<AdministratorScheme>>> HandleAsync(AdministratorFiltersParameters parameters, CancellationToken cancellation = default)
    {
        var pagination = PaginationFilters.From(parameters.Page, parameters.PageSize);
        var sort = SortFilters.From(parameters.By, Enum.Parse<SortDirection>(parameters.Order ?? SortDirection.Descending.ToString(), ignoreCase: true));

        var filters = AdministratorFilters.AsBuilder()
            .WithIdentifier(parameters.Id)
            .WithName(parameters.Name)
            .WithCode(parameters.Code)
            .WithPagination(pagination)
            .WithSort(sort)
            .WithIsDeleted(false)
            .Build();

        var administrators = await repository.FilterAdministratorsAsync(filters, cancellation);
        var total = await repository.CountAdministratorsAsync(filters, cancellation);

        var result = new Pagination<AdministratorScheme>()
            .WithItems([.. administrators.Select(administrator => mapper.Map<AdministratorScheme>(administrator))])
            .WithTotal((int)total)
            .WithPageNumber(parameters.Page)
            .WithPageSize(parameters.PageSize);

        return Result<Pagination<AdministratorScheme>>.Success(result);
    }
}
