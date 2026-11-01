namespace Apta.Infrastructure.Persistence.Repositories;

public abstract class AggregateRepository<TAggregate>(DbContext database, ILogger<AggregateRepository<TAggregate>> logger) : IAggregateCollection<TAggregate>
    where TAggregate : Aggregate
{
    public virtual async Task<TAggregate> InsertAsync(TAggregate aggregate, InsertionBehavior behavior = InsertionBehavior.FailIfExists, CancellationToken cancellation = default)
    {
        aggregate.Id = Identifier.Generate<TAggregate>();
        aggregate.CreatedAt = DateTime.UtcNow;

        await database.Set<TAggregate>().AddAsync(aggregate, cancellation);

        logger.LogInformation("Inserting aggregate of type {AggregateType} with ID {AggregateId}", typeof(TAggregate).Name, aggregate.Id);
        logger.LogInformation("Inserting aggregate with details: {@Aggregate}", aggregate);

        return aggregate;
    }

    public virtual async Task InsertManyAsync(IEnumerable<TAggregate> aggregates, InsertionBehavior behavior = InsertionBehavior.FailIfExists, CancellationToken cancellation = default)
    {
        foreach (var aggregate in aggregates)
        {
            aggregate.Id = Identifier.Generate<TAggregate>();
            aggregate.CreatedAt = DateTime.UtcNow;
        }

        logger.LogInformation("Inserting aggregates of type {AggregateType}.", typeof(TAggregate).Name);
        logger.LogInformation("Total aggregates to insert: {AggregateCount}.", aggregates.Count());

        await database.Set<TAggregate>().AddRangeAsync(aggregates, cancellation);
    }

    public virtual async Task<bool> DeleteAsync(TAggregate aggregate, DeletionBehavior behavior = DeletionBehavior.Soft, CancellationToken cancellation = default)
    {
        logger.LogInformation("Deleting aggregate of type {AggregateType} with ID {AggregateId}", typeof(TAggregate).Name, aggregate.Id);
        logger.LogInformation("Deleting {AggregateId} with deletion behavior: {DeletionBehavior}", aggregate.Id, behavior);

        switch (behavior)
        {
            /* marks the record as deleted without physically removing it from the database (soft delete) */
            case DeletionBehavior.Soft:
                {
                    aggregate.MarkAsDeleted();
                    aggregate.MarkAsUpdated();

                    database.Set<TAggregate>().Update(aggregate);
                    return true;
                }

            /* permanently removes the record from the database (physical delete) */
            case DeletionBehavior.Hard:
                {
                    database.Set<TAggregate>().Remove(aggregate);
                    return true;
                }
        }

        return false;
    }

    public virtual async Task<TAggregate> UpdateAsync(TAggregate aggregate, CancellationToken cancellation = default)
    {
        var tracked = await database.Set<TAggregate>().FirstAsync(where => where.Id == aggregate.Id, cancellation);

        aggregate.MarkAsUpdated();

        logger.LogInformation("Updating aggregate of type {AggregateType} with ID {AggregateId}", typeof(TAggregate).Name, aggregate.Id);
        logger.LogInformation("Updating aggregate with details: {@Aggregate}", aggregate);

        database.Entry(tracked).CurrentValues
            .SetValues(aggregate);

        return tracked;
    }
}
