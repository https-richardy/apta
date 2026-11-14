namespace Apta.Domain.Aggregates;

public sealed class Customer : Aggregate
{
    public string Name { get; set; } = default!;
    public string Phone { get; set; } = default!;

    public string Document { get; set; } = default!;
}
