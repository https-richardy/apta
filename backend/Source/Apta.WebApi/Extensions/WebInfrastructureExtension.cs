namespace Apta.WebApi.Extensions;

[ExcludeFromCodeCoverage(Justification = "does not contain any logic that can be tested.")]
public static class WebInfrastructureExtension
{
    public static void AddWebInfrastructure(this IServiceCollection services)
    {
        services.Configure<KestrelServerOptions>(options => options.Limits.MaxRequestBodySize = 32 * 1024 * 1024);
        services.Configure<FormOptions>(options => options.MultipartBodyLengthLimit = 32 * 1024 * 1024);

        services.AddOpenApi();
        services.AddControllers(options =>
        {
            options.ModelBinderProviders.Insert(0, new BlobModelBinderProvider());
        });

        services.AddCorsConfiguration();
        services.AddHttpContextAccessor();
        services.AddEndpointsApiExplorer();
        services.AddBearerAuthentication();
        services.AddAuthorization();

        services.AddTransformers();
        services.AddAuthorization();
        services.AddFluentValidationAutoValidation(options =>
        {
            options.DisableDataAnnotationsValidation = true;
        });
    }
}
