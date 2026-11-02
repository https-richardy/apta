#pragma warning disable IDE0060

namespace Apta.WebApi.Metadata;

[ExcludeFromCodeCoverage(Justification = "contains ASP.NET Core api convention metadata only; methods are never executed directly.")]
public static class AdministratorsMetadata
{
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    [ProducesResponseType(typeof(AdministratorScheme), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public static void CreateAdministratorAsync(AdministratorCreationScheme request, CancellationToken cancellation) { }
}
