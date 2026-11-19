namespace Apta.Application.Handlers.Customers;

public sealed class CustomersImportHandler(IEnqueuer<CustomersImportScheme> enqueuer) :
    IDispatchHandler<CustomersImportScheme, Result>
{
    public async Task<Result> HandleAsync(CustomersImportScheme parameters, CancellationToken cancellation = default)
    {
        if (parameters.File.Stream.CanSeek && parameters.File.Stream.Length == 0)
        {
            /* the stream won't be used anymore, so we can discard it */
            /* https://learn.microsoft.com/en-us/dotnet/api/system.io.stream.dispose */

            await parameters.File.Stream.DisposeAsync();

            return Result.Failure(CustomerErrors.NoFileContent);
        }

        await enqueuer.EnqueueAsync(parameters, cancellation);

        return Result.Success();
    }
}
