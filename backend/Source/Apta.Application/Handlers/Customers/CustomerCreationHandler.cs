namespace Apta.Application.Handlers.Customers;

public sealed class CustomerCreationHandler(ICustomerRepository repository, IUnitOfWork unitOfWork, IMapper mapper) :
    IDispatchHandler<CustomerCreationScheme, Result<CustomerScheme>>
{
    public async Task<Result<CustomerScheme>> HandleAsync(CustomerCreationScheme parameters, CancellationToken cancellation = default)
    {
        var filter = CustomerFilters.AsBuilder()
            .WithDocument(parameters.Document)
            .WithIsDeleted(false)
            .Build();

        var customers = await repository.FilterCustomersAsync(filter, cancellation);
        var existing = customers.FirstOrDefault();

        if (existing is not null)
        {
            return Result<CustomerScheme>.Failure(CustomerErrors.AlreadyExists);
        }

        var customer = mapper.Map<Customer>(parameters);
        var details = await unitOfWork.TransactionAsync(cancellation, async cancellationToken =>
        {
            var persistedCustomer = await repository.InsertAsync(customer, cancellation: cancellationToken);

            return mapper.Map<CustomerScheme>(persistedCustomer);
        });

        return Result<CustomerScheme>.Success(details);
    }
}
