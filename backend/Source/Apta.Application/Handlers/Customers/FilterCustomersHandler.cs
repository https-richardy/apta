namespace Apta.Application.Handlers.Customers;

public sealed class FilterCustomersHandler(ICustomerRepository repository, IMapper mapper) :
    IDispatchHandler<CustomerFiltersParameters, Result<Pagination<CustomerScheme>>>
{
    public async Task<Result<Pagination<CustomerScheme>>> HandleAsync(CustomerFiltersParameters parameters, CancellationToken cancellation = default)
    {
        var pagination = PaginationFilters.From(parameters.Page, parameters.PageSize);
        var sort = SortFilters.From(parameters.By, Enum.Parse<SortDirection>(parameters.Order ?? SortDirection.Descending.ToString(), ignoreCase: true));

        var filters = CustomerFilters.AsBuilder()
            .WithIdentifier(parameters.Id)
            .WithName(parameters.Name)
            .WithPhone(parameters.Phone)
            .WithEmail(parameters.Email)
            .WithDocument(parameters.Document)
            .WithPagination(pagination)
            .WithSort(sort)
            .WithIsDeleted(false)
            .Build();

        var customers = await repository.FilterCustomersAsync(filters, cancellation);
        var total = await repository.CountCustomersAsync(filters, cancellation);

        var result = new Pagination<CustomerScheme>()
            .WithItems([.. customers.Select(customer => mapper.Map<CustomerScheme>(customer))])
            .WithTotal((int)total)
            .WithPageNumber(parameters.Page)
            .WithPageSize(parameters.PageSize);

        return Result<Pagination<CustomerScheme>>.Success(result);
    }
}
