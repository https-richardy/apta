namespace Apta.Application.Transport.Administrators;

public sealed record AdministratorScheme
{
    public string Id { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string Code { get; init; } = default!;

    public DateTime CreatedAt { get; init; } = default!;
    public DateTime UpdatedAt { get; init; } = default!;
}
