namespace Apta.Domain.Aggregates;

public sealed class Administrator : Aggregate
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
}
