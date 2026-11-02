namespace Apta.Domain.Filtering.Builders;

public sealed class AdministratorFiltersBuilder :
    FiltersBuilderBase<AdministratorFilters, AdministratorFiltersBuilder>
{
    public AdministratorFiltersBuilder WithName(string? name)
    {
        if (!string.IsNullOrWhiteSpace(name))
            _filters.Name = name.Trim();

        return this;
    }

    public AdministratorFiltersBuilder WithCode(string? code)
    {
        if (!string.IsNullOrWhiteSpace(code))
            _filters.Code = code.Trim();

        return this;
    }
}
