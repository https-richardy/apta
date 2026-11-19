namespace Apta.Infrastructure.Workers;

public sealed class ImportCustomersWorker(ICustomerRepository repository, ILogger<ImportCustomersWorker> logger) :
    IQueueHandler<CustomersImportScheme>
{
    public async Task HandleAsync(CustomersImportScheme context, CancellationToken cancellation = default)
    {
        logger.LogInformation("initializing customer import process.");

        await repository.InsertManyAsync([], cancellation: cancellation);
    }
}
