namespace Apta.WebUI.Transport.Customers;

public sealed record CustomersImportScheme
{
    public Stream Content { get; set; } = default!;
    public String FileName { get; set; } = default!;
}
