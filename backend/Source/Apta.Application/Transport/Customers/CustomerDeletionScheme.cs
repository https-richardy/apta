namespace Apta.Application.Transport.Customers;

public sealed record CustomerDeletionScheme : IDispatchable<Result>
{
    [Description("the unique identifier of the customer to delete, for example, 'customer_d3TjZUiCvu32Uq2LiapbNWjKa'")]
    public string Id { get; init; } = default!;
}
