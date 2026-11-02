namespace Apta.Domain.Filtering;

public sealed class AdministratorFilters : Filters
{
    public string? Name { get; set; }
    public string? Code { get; set; }

    public static AdministratorFilters WithoutFilters => new();
    public static AdministratorFiltersBuilder AsBuilder() => new();
}
