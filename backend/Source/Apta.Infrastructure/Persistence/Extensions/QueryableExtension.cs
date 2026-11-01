namespace Apta.Infrastructure.Persistence.Extensions;

public static class QueryableExtension
{
    public static IQueryable<TAggregate> FilterWhenNotNull<TAggregate>(this IQueryable<TAggregate> query, bool condition, Expression<Func<TAggregate, bool>> predicate)
    {
        return condition
            ? query.Where(predicate)
            : query;
    }

    public static IQueryable<TAggregate> FilterWhenNotNull<TAggregate>(this IQueryable<TAggregate> query, string? value, Expression<Func<TAggregate, string>> property)
    {
        if (string.IsNullOrWhiteSpace(value))
            return query;

        var parameter = property.Parameters[0];
        var body = Expression.Call(
            property.Body,
            nameof(string.Contains),
            Type.EmptyTypes,
            Expression.Constant(value)
        );

        var lambda = Expression.Lambda<Func<TAggregate, bool>>(body, parameter);

        return query.Where(lambda);
    }

    public static IQueryable<TAggregate> FilterWhenNotNull<TAggregate, TValue>(this IQueryable<TAggregate> query, TValue? value, Expression<Func<TAggregate, TValue>> property)
        where TValue : struct
    {
        if (!value.HasValue)
            return query;

        var parameter = property.Parameters[0];
        var body = Expression.Equal(
            left: property.Body,
            right: Expression.Constant(value.Value)
        );

        var lambda = Expression.Lambda<Func<TAggregate, bool>>(body, parameter);

        return query.Where(lambda);
    }
}
