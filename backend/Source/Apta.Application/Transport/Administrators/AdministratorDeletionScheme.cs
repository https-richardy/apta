namespace Apta.Application.Transport.Administrators;

public sealed record AdministratorDeletionScheme : IDispatchable<Result>
{
    [Description("the unique identifier of the administrator to delete, for example, 'administrator_d3TjZUiCvu32Uq2LiapbNWjKa'")]
    public string Id { get; init; } = default!;
}
