namespace Apta.Domain.Filtering.Builders;

public sealed class CustomerFiltersBuilder :
    FiltersBuilderBase<CustomerFilters, CustomerFiltersBuilder>
{
    public CustomerFiltersBuilder WithName(string? name)
    {
        if (!string.IsNullOrWhiteSpace(name))
            _filters.Name = name.Trim();

        return this;
    }

    public CustomerFiltersBuilder WithPhone(string? phone)
    {
        if (!string.IsNullOrWhiteSpace(phone))
            _filters.Phone = phone.Trim();

        return this;
    }

    public CustomerFiltersBuilder WithEmail(string? email)
    {
        if (!string.IsNullOrWhiteSpace(email))
            _filters.Email = email.Trim();

        return this;
    }

    public CustomerFiltersBuilder WithDocument(string? document)
    {
        var digits = new string([.. (document ?? string.Empty).Where(char.IsDigit)]);

        if (!string.IsNullOrEmpty(digits))
            _filters.Document = digits;

        return this;
    }
}
