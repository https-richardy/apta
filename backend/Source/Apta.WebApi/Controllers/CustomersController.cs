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

    [HttpPut("{id}")]
    [Authorize]
    [EndpointDescription("updates a customer identified by its unique identifier")]
    public async Task<IActionResult> UpdateCustomerAsync([FromRoute] string id, [FromBody] UpdateCustomerScheme request, CancellationToken cancellation)
    {
        var result = await dispatcher.DispatchAsync(request with { Id = id }, cancellation);

        return result switch
        {
            { IsSuccess: true } when result.Data is not null =>
                StatusCode(StatusCodes.Status200OK, result.Data),

            { IsFailure: true } when result.Error == CustomerErrors.DoesNotExist =>
                StatusCode(StatusCodes.Status404NotFound, result.Error),

            { IsFailure: true } when result.Error == CustomerErrors.AlreadyExists =>
                StatusCode(StatusCodes.Status409Conflict, result.Error),
        };
    }

    [HttpDelete("{id}")]
    [Authorize]
    [EndpointDescription("deletes a customer identified by its unique identifier")]
    public async Task<IActionResult> DeleteCustomerAsync([FromRoute] CustomerDeletionScheme request, CancellationToken cancellation)
    {
        var result = await dispatcher.DispatchAsync(request, cancellation);

        return result switch
        {
            { IsSuccess: true } =>
                StatusCode(StatusCodes.Status204NoContent),

            { IsFailure: true } when result.Error == CustomerErrors.DoesNotExist =>
                StatusCode(StatusCodes.Status404NotFound, result.Error),
        };
    }

    [HttpPost("import")]
    [Authorize]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [EndpointDescription("imports customers in batch from a csv file, processed asynchronously through the queue")]
    public async Task<IActionResult> ImportCustomersAsync([FromForm] CustomersImportScheme request, CancellationToken cancellation)
    {
        var result = await dispatcher.DispatchAsync(request, cancellation);

        return result switch
        {
            { IsSuccess: true } =>
                StatusCode(StatusCodes.Status202Accepted),

            { IsFailure: true } when result.Error == CustomerErrors.NoFileContent =>
                StatusCode(StatusCodes.Status422UnprocessableEntity, result.Error),
        };
    }
}
