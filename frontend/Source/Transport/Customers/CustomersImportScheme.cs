namespace Apta.WebUI.Transport.Customers;

public sealed record CustomersImportScheme
{
    public Stream Content { get; init; } = default!;
    public String FileName { get; init; } = default!;
}
