#pragma warning disable IDE0060

namespace Apta.WebApi.Metadata;

[ExcludeFromCodeCoverage(Justification = "contains ASP.NET Core api convention metadata only; methods are never executed directly.")]
public static class AdministratorsMetadata
{
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    [ProducesResponseType(typeof(Pagination<AdministratorScheme>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public static void FilterAdministratorsAsync(AdministratorFiltersParameters request, CancellationToken cancellation) { }

    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    [ProducesResponseType(typeof(AdministratorScheme), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public static void CreateAdministratorAsync(AdministratorCreationScheme request, CancellationToken cancellation) { }

    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    [ProducesResponseType(typeof(AdministratorScheme), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public static void UpdateAdministratorAsync(string id, UpdateAdministratorScheme request, CancellationToken cancellation) { }
}
