namespace Apta.Infrastructure.Workers;

public sealed class ImportCustomersWorker(ICustomerRepository repository, IUnitOfWork unitOfWork, ICustomerParser parser, ILogger<ImportCustomersWorker> logger) :
    IQueueHandler<CustomersImportScheme>
{
    public async Task HandleAsync(CustomersImportScheme context, CancellationToken cancellation = default)
    {
        var result = await parser.ParseCustomersAsync(context.File.Stream, cancellation);
        if (result.IsFailure || result.Data is null)
        {
            logger.LogWarning("customer import aborted while parsing the file: {Error}.", result.Error);
            return;
        }

        if (result.Data.Count == 0)
        {
            logger.LogWarning("customer import finished with no rows to persist.");
            return;
        }

        var seen = new HashSet<string>();
        var customers = result.Data
            .Where(customer => string.IsNullOrWhiteSpace(customer.Document) || seen.Add(customer.Document))
            .ToList();

        await unitOfWork.TransactionAsync(cancellation, async cancellationToken =>
        {
            await repository.InsertManyAsync(customers, cancellation: cancellationToken);
        });

        logger.LogInformation("customer import finished, {Count} customers persisted.", customers.Count);
    }
}
