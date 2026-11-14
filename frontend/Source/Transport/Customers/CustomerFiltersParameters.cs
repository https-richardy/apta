namespace Apta.WebUI.Transport.Customers;

public sealed record CustomerFiltersParameters
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Document { get; set; }

    public string? By { get; set; }
    public string? Order { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
