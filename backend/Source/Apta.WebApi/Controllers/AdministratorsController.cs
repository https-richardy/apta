namespace Apta.WebApi.Controllers;

[Route("api/v1/administrators")]
[ApiController]
[ApiConventionType(typeof(AdministratorsMetadata))]
public sealed class AdministratorsController(IDispatcher dispatcher) : ControllerBase
{
    [HttpPost]
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
}
