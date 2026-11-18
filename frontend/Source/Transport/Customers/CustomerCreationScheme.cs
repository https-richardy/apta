namespace Apta.WebUI.Transport.Customers;

public sealed record CustomerCreationScheme
{
    public string Name { get; set; } = default!;
    public string? Phone { get; set; }
    public string? Document { get; set; }
}
