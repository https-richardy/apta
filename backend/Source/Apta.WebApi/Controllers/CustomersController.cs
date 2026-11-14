namespace Apta.WebApi.Controllers;

[Route("api/v1/customers")]
[ApiController]
[ApiConventionType(typeof(CustomersMetadata))]
public sealed class CustomersController(IDispatcher dispatcher) : ControllerBase
{
    [HttpGet]
    [Authorize]
    [EndpointDescription("searches for customers based on the provided filters")]
    public async Task<IActionResult> FilterCustomersAsync([FromQuery] CustomerFiltersParameters request, CancellationToken cancellation)
    {
        var result = await dispatcher.DispatchAsync(request, cancellation);

        return result switch
        {
            { IsSuccess: true } => StatusCode(StatusCodes.Status200OK, result.Data),
        };
    }

    [HttpPost]
    [Authorize]
    [EndpointDescription("creates a customer from the provided data")]
    public async Task<IActionResult> CreateCustomerAsync([FromBody] CustomerCreationScheme request, CancellationToken cancellation)
    {
        var result = await dispatcher.DispatchAsync(request, cancellation);

        return result switch
        {
            { IsSuccess: true } when result.Data is not null =>
                StatusCode(StatusCodes.Status201Created, result.Data),

            { IsFailure: true } when result.Error == CustomerErrors.AlreadyExists =>
                StatusCode(StatusCodes.Status409Conflict, result.Error),
        };
    }
}
