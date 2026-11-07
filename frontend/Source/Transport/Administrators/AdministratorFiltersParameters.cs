namespace Apta.WebUI.Transport.Administrators;

public sealed record AdministratorFiltersParameters
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }

    public string? By { get; set; }
    public string? Order { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
