namespace Apta.Application.Handlers.Customers;

public sealed class CustomerUpdateHandler(ICustomerRepository repository, IUnitOfWork unitOfWork, IMapper mapper) :
    IDispatchHandler<UpdateCustomerScheme, Result<CustomerScheme>>
{
    public async Task<Result<CustomerScheme>> HandleAsync(UpdateCustomerScheme parameters, CancellationToken cancellation = default)
    {
        var filters = CustomerFilters.AsBuilder()
            .WithIdentifier(parameters.Id)
            .Build();

        var customers = await repository.FilterCustomersAsync(filters, cancellation);
        var customer = customers.FirstOrDefault();

        if (customer is null)
        {
            return Result<CustomerScheme>.Failure(CustomerErrors.DoesNotExist);
        }

        var document = (parameters.Document ?? string.Empty).SanitizeNumbers();
        var duplicateFilters = CustomerFilters.AsBuilder()
            .WithDocument(document)
            .Build();

        var duplicates = string.IsNullOrWhiteSpace(document)
            ? Enumerable.Empty<Customer>()
            : await repository.FilterCustomersAsync(duplicateFilters, cancellation);

        if (duplicates.Any(other => other.Document == document && other.Id != customer.Id))
        {
            return Result<CustomerScheme>.Failure(CustomerErrors.AlreadyExists);
        }

        mapper.Map(parameters, customer);

        var details = await unitOfWork.TransactionAsync(cancellation, async cancellationToken =>
        {
            var persistedCustomer = await repository.UpdateAsync(customer, cancellationToken);

            return mapper.Map<CustomerScheme>(persistedCustomer);
        });

        return Result<CustomerScheme>.Success(details);
    }
}
