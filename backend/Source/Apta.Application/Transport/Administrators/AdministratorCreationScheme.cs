namespace Apta.Application.Transport.Administrators;

public sealed record AdministratorCreationScheme : IDispatchable<Result<AdministratorScheme>>
{
    [Description("the trade name of the consortium administrator authorized by the brazilian central bank, for example, 'EMBRACON'")]
    public string Name { get; init; } = default!;

    [Description("the BACEN code (ISBP, 8 digits) of the consortium administrator, for example, '58113812'")]
    public string Code { get; init; } = default!;
}
