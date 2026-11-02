namespace Apta.WebApi.Extensions;

[ExcludeFromCodeCoverage(Justification = "does not contain any logic that can be tested.")]
public static class HttpPipelineExtension
{
    public static void UseMiddlewares(this IApplicationBuilder app)
    {
        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}