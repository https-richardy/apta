namespace Apta.WebApi.Extensions;

[ExcludeFromCodeCoverage(Justification = "does not contain any logic that can be tested.")]
public static class WebInfrastructureExtension
{
    public static void AddWebInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var maxRequestBodySize = configuration.GetValue("Upload:MaxRequestBodySizeInBytes", 32L * 1024 * 1024);

        services.Configure<KestrelServerOptions>(options => options.Limits.MaxRequestBodySize = maxRequestBodySize);
        services.Configure<FormOptions>(options => options.MultipartBodyLengthLimit = maxRequestBodySize);

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
