namespace Apta.Domain.Filtering;

public sealed class CustomerFilters : Filters
{
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Document { get; set; }

    public static CustomerFilters WithoutFilters => new();
    public static CustomerFiltersBuilder AsBuilder() => new();
}
