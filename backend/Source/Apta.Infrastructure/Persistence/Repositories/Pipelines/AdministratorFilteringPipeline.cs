namespace Apta.Infrastructure.Persistence.Repositories.Pipelines;

public static class AdministratorFilteringPipeline
{
    public static IQueryable<Administrator> FilterAdministrators(this IQueryable<Administrator> query, AdministratorFilters filters)
    {
        return filters is null ? query : query
            .FilterWhenNotNull(filters.Name, administrator => administrator.Name)
            .FilterWhenNotNull(filters.Code, administrator => administrator.Code);
    }
}
