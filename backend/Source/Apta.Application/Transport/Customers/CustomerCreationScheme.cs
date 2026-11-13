namespace Apta.Application.Transport.Customers;

public sealed record CustomerCreationScheme : IDispatchable<Result<CustomerScheme>>
{
    [Description("the full name of the customer, for example, 'João da Silva'")]
    public string Name { get; init; } = default!;

    [Description("the customer's phone number, optional, for example, '(11) 98888-7777'")]
    public string? Phone { get; init; }

    [Description("the customer's email address, optional, for example, 'joao.silva@gmail.com'")]
    public string? Email { get; init; }

    [Description("the customer's CPF (brazilian individual registry document), optional, for example, '123.456.789-09'")]
    public string? Document { get; init; }
}
