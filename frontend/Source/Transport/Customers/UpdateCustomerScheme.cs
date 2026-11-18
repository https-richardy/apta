namespace Apta.WebUI.Transport.Customers;

public sealed record UpdateCustomerScheme
{
    [JsonIgnore]
    public string Id { get; set; } = default!;

    public string Name { get; set; } = default!;
    public string? Phone { get; set; }
    public string? Document { get; set; }
}
