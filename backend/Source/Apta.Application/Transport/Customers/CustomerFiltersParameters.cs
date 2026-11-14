namespace Apta.Application.Transport.Customers;

public sealed record CustomerFiltersParameters : IDispatchable<Result<Pagination<CustomerScheme>>>
{
    [Description("the unique identifier of the customer, for example, 'customer_d3TjZUiCvu32Uq2LiapbNWjKa'")]
    public string? Id { get; init; }

    [Description("the partial or full name of the customer, for example, 'João'")]
    public string? Name { get; init; }

    [Description("the partial or full phone number of the customer, for example, '11988887777'")]
    public string? Phone { get; init; }

    [Description("the partial or full CPF (brazilian individual registry document) of the customer, for example, '12345678909'")]
    public string? Document { get; init; }

    [Description("the field to sort the customers by, for example, 'Name' or 'CreatedAt'")]
    public string? By { get; init; }

    [Description("the sort direction, either 'Ascending' or 'Descending'")]
    public string? Order { get; init; }

    [Description("the current page number, for example, 1")]
    public int Page { get; init; } = 1;

    [Description("the number of items per page, for example, 10")]
    public int PageSize { get; init; } = 10;
}
