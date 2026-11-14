#pragma warning disable IDE0060

namespace Apta.WebApi.Metadata;

[ExcludeFromCodeCoverage(Justification = "contains ASP.NET Core api convention metadata only; methods are never executed directly.")]
public static class CustomersMetadata
{
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    [ProducesResponseType(typeof(Pagination<CustomerScheme>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public static void FilterCustomersAsync(CustomerFiltersParameters request, CancellationToken cancellation) { }

    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    [ProducesResponseType(typeof(CustomerScheme), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public static void CreateCustomerAsync(CustomerCreationScheme request, CancellationToken cancellation) { }
}
