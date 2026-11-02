namespace Apta.Infrastructure.Persistence.Extensions;

[ExcludeFromCodeCoverage(Justification = "only builds linq expression trees that are translated to sql by the database provider, so it does not contain any logic that can be unit tested; its behavior is covered by the persistence integration tests.")]
public static class SqlQueryableExtension
{
    public static IQueryable<TAggregate> Paginate<TAggregate>(this IQueryable<TAggregate> queryable, PaginationFilters? filters)
    {
        if (filters is null || filters.PageNumber <= 0 || filters.PageSize <= 0)
            return queryable;

        var skip = (filters.PageNumber - 1) * filters.PageSize;

        return queryable
            .Skip(skip)
            .Take(filters.PageSize);
    }

    public static IQueryable<TAggregate> Sort<TAggregate>(this IQueryable<TAggregate> query, SortFilters? filters)
    {
        if (filters is null || string.IsNullOrWhiteSpace(filters.Field))
            return query;

        var parameter = Expression.Parameter(typeof(TAggregate), "aggregate");
        var property = Expression.PropertyOrField(parameter, filters.Field);
        var lambda = Expression.Lambda(property, parameter);

        var method = filters.Direction == SortDirection.Ascending
            ? "OrderBy"
            : "OrderByDescending";

        var result = Expression.Call(typeof(Queryable), method, [typeof(TAggregate), property.Type], query.Expression, Expression.Quote(lambda));

        return query.Provider.CreateQuery<TAggregate>(result);
    }
}
