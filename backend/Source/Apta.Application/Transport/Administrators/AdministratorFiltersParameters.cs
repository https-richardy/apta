namespace Apta.Application.Transport.Administrators;

public sealed record AdministratorFiltersParameters : IDispatchable<Result<Pagination<AdministratorScheme>>>
{
    [Description("the unique identifier of the administrator, for example, 'administrator_d3TjZUiCvu32Uq2LiapbNWjKa'")]
    public string? Id { get; init; }

    [Description("the partial or full trade name of the administrator, for example, Embracon")]
    public string? Name { get; init; }

    [Description("the BACEN code (ISPB, 8 digits) of the administrator, for example, '58113812'")]
    public string? Code { get; init; }

    [Description("the field to sort the administrators by, for example, 'Name' or 'CreatedAt'")]
    public string? By { get; init; }

    [Description("the sort direction, either 'Ascending' or 'Descending'")]
    public string? Order { get; init; }

    [Description("the current page number, for example, 1")]
    public int Page { get; init; } = 1;

    [Description("the number of items per page, for example, 10")]
    public int PageSize { get; init; } = 10;
}
