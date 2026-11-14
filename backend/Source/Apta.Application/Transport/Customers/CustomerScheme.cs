namespace Apta.Application.Transport.Customers;

public sealed record CustomerScheme
{
    public string Id { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string Phone { get; init; } = default!;
    public string Document { get; init; } = default!;

    public DateTime CreatedAt { get; init; } = default!;
    public DateTime UpdatedAt { get; init; } = default!;
}
