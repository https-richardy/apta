namespace Apta.WebApi.Extensions;

[ExcludeFromCodeCoverage(Justification = "does not contain any logic that can be tested.")]
public static class TransformersExtension
{
    public static void AddTransformers(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddScalarTransformers();
            options.AddDocumentTransformer<SecurityTransformer>();
            options.AddDocumentTransformer<DescriptionTransformer>();
        });
    }
}
