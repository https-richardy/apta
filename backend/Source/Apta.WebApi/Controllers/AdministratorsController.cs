namespace Apta.WebApi.Controllers;

[Route("api/v1/administrators")]
[ApiController]
[ApiConventionType(typeof(AdministratorsMetadata))]
public sealed class AdministratorsController(IDispatcher dispatcher) : ControllerBase
{
    [HttpGet]
    [Authorize]
    [EndpointDescription("searches for administrators based on the provided filters")]
    public async Task<IActionResult> FilterAdministratorsAsync([FromQuery] AdministratorFiltersParameters request, CancellationToken cancellation)
    {
        var result = await dispatcher.DispatchAsync(request, cancellation);

        return result switch
        {
            { IsSuccess: true } => StatusCode(StatusCodes.Status200OK, result.Data),
        };
    }

    [HttpPost]
    [Authorize]
    [EndpointDescription("creates an administrator from the provided data")]
    public async Task<IActionResult> CreateAdministratorAsync([FromBody] AdministratorCreationScheme request, CancellationToken cancellation)
    {
        var result = await dispatcher.DispatchAsync(request, cancellation);

        return result switch
        {
            { IsSuccess: true } when result.Data is not null =>
                StatusCode(StatusCodes.Status201Created, result.Data),

            { IsFailure: true } when result.Error == AdministratorErrors.AlreadyExists =>
                StatusCode(StatusCodes.Status409Conflict, result.Error),
        };
    }

    [HttpPut("{id}")]
    [Authorize]
    [EndpointDescription("updates an administrator identified by its unique identifier")]
    public async Task<IActionResult> UpdateAdministratorAsync([FromRoute] string id, [FromBody] UpdateAdministratorScheme request, CancellationToken cancellation)
    {
        var result = await dispatcher.DispatchAsync(request with { Id = id }, cancellation);

        return result switch
        {
            { IsSuccess: true } when result.Data is not null =>
                StatusCode(StatusCodes.Status200OK, result.Data),

            { IsFailure: true } when result.Error == AdministratorErrors.DoesNotExist =>
                StatusCode(StatusCodes.Status404NotFound, result.Error),

            { IsFailure: true } when result.Error == AdministratorErrors.AlreadyExists =>
                StatusCode(StatusCodes.Status409Conflict, result.Error),
        };
    }

    [HttpDelete("{id}")]
    [Authorize]
    [EndpointDescription("deletes an administrator identified by its unique identifier")]
    public async Task<IActionResult> DeleteAdministratorAsync([FromRoute] AdministratorDeletionScheme request, CancellationToken cancellation)
    {
        var result = await dispatcher.DispatchAsync(request, cancellation);

        return result switch
        {
            { IsSuccess: true } =>
                StatusCode(StatusCodes.Status204NoContent),

            { IsFailure: true } when result.Error == AdministratorErrors.DoesNotExist =>
                StatusCode(StatusCodes.Status404NotFound, result.Error),
        };
    }
}
