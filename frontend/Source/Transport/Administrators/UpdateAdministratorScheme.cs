namespace Apta.WebUI.Transport.Administrators;

public sealed record UpdateAdministratorScheme
{
    [JsonIgnore]
    public string Id { get; set; } = default!;

    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
}
