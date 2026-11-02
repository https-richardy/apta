namespace Apta.Infrastructure.Persistence.Repositories.Pipelines;

public static class CustomerFilteringPipeline
{
    public static IQueryable<Customer> FilterCustomers(this IQueryable<Customer> query, CustomerFilters filters)
    {
        return filters is null ? query : query
            .FilterWhenNotNull(filters.Name, customer => customer.Name)
            .FilterWhenNotNull(filters.Phone, customer => customer.Phone)
            .FilterWhenNotNull(filters.Document, customer => customer.Document);
    }
}
