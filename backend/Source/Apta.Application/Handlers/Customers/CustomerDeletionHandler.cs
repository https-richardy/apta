namespace Apta.Application.Handlers.Customers;

public sealed class CustomerDeletionHandler(ICustomerRepository repository, IUnitOfWork unitOfWork) :
    IDispatchHandler<CustomerDeletionScheme, Result>
{
    public async Task<Result> HandleAsync(CustomerDeletionScheme parameters, CancellationToken cancellation = default)
    {
        var filters = CustomerFilters.AsBuilder()
            .WithIdentifier(parameters.Id)
            .Build();

        var customers = await repository.FilterCustomersAsync(filters, cancellation);
        var customer = customers.FirstOrDefault();

        if (customer is null)
        {
            return Result.Failure(CustomerErrors.DoesNotExist);
        }

        await unitOfWork.TransactionAsync(cancellation, async cancellationToken =>
        {
            await repository.DeleteAsync(customer, cancellation: cancellationToken);
        });

        return Result.Success();
    }
}
