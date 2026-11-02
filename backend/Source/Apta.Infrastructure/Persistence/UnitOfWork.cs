namespace Apta.Infrastructure.Persistence;

public sealed class UnitOfWork(DatabaseContext database, ILogger<UnitOfWork> logger) : IUnitOfWork
{
    private readonly DatabaseContext _database = database;
    private readonly ILogger<UnitOfWork> _logger = logger;

    public async Task TransactionAsync(CancellationToken cancellation, Func<CancellationToken, Task> action)
    {
        var strategy = _database.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async token =>
        {
            await using var transaction = await _database.Database.BeginTransactionAsync(token);

            try
            {
                _logger.LogInformation("Transaction started with ID: {TransactionId}", transaction.TransactionId);

                await action(token);

                await _database.SaveChangesAsync(token);
                await transaction.CommitAsync(token);

                _logger.LogInformation("Transaction committed with ID: {TransactionId}", transaction.TransactionId);
            }
            catch
            {
                _logger.LogWarning("Transaction rolled back with ID: {TransactionId}", transaction.TransactionId);

                await transaction.RollbackAsync(token);
                throw;
            }
        }, cancellation);
    }

    public async Task<TResult> TransactionAsync<TResult>(CancellationToken cancellation, Func<CancellationToken, Task<TResult>> action)
    {
        var strategy = _database.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async token =>
        {
            await using var transaction = await _database.Database.BeginTransactionAsync(token);

            try
            {
                _logger.LogInformation("Transaction started with ID: {TransactionId}", transaction.TransactionId);

                TResult result = await action(token);

                await _database.SaveChangesAsync(token);
                await transaction.CommitAsync(token);

                _logger.LogInformation("Transaction committed with ID: {TransactionId}", transaction.TransactionId);

                return result;
            }
            catch
            {
                _logger.LogWarning("Transaction rolled back with ID: {TransactionId}", transaction.TransactionId);

                await transaction.RollbackAsync(token);
                throw;
            }
        }, cancellation);
    }
}
