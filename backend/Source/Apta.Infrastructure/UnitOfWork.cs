namespace Apta.Infrastructure;

public sealed class UnitOfWork(DatabaseContext database, ILogger<UnitOfWork> logger) : IUnitOfWork
{
    private readonly DatabaseContext _database = database;
    private readonly ILogger<UnitOfWork> _logger = logger;

    public async Task TransactionAsync(CancellationToken cancellation, Func<CancellationToken, Task> action)
    {
        await using var transaction = await _database.Database.BeginTransactionAsync(cancellation);

        try
        {
            _logger.LogInformation("Transaction started with ID: {TransactionId}", transaction.TransactionId);

            await action(cancellation);

            await _database.SaveChangesAsync(cancellation);
            await transaction.CommitAsync(cancellation);

            _logger.LogInformation("Transaction committed with ID: {TransactionId}", transaction.TransactionId);
        }
        catch
        {
            _logger.LogWarning("Transaction rolled back with ID: {TransactionId}", transaction.TransactionId);

            await transaction.RollbackAsync(cancellation);
            throw;
        }
    }

    public async Task<TResult> TransactionAsync<TResult>(CancellationToken cancellation, Func<CancellationToken, Task<TResult>> action)
    {
        await using var transaction = await _database.Database.BeginTransactionAsync(cancellation);

        try
        {
            _logger.LogInformation("Transaction started with ID: {TransactionId}", transaction.TransactionId);

            TResult result = await action(cancellation);

            await _database.SaveChangesAsync(cancellation);
            await transaction.CommitAsync(cancellation);

            _logger.LogInformation("Transaction committed with ID: {TransactionId}", transaction.TransactionId);

            return result;
        }
        catch
        {
            _logger.LogWarning("Transaction rolled back with ID: {TransactionId}", transaction.TransactionId);

            await transaction.RollbackAsync(cancellation);
            throw;
        }
    }
}