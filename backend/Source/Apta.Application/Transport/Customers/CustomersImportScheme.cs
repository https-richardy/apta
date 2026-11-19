namespace Apta.Application.Transport.Customers;

public sealed record CustomersImportScheme : IDispatchable<Result>
{
    [Description("the csv file containing the customers to be imported in batch")]
    public Blob File { get; init; } = default!;
}
