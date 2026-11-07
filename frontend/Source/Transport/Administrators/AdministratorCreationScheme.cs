namespace Apta.WebUI.Transport.Administrators;

public sealed record AdministratorCreationScheme
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
}
