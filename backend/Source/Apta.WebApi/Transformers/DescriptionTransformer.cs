namespace Apta.WebApi.Transformers;

[ExcludeFromCodeCoverage(Justification = "does not contain any logic that can be tested.")]
public sealed class DescriptionTransformer : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var overview = Path.Combine(AppContext.BaseDirectory, "Metadata", "OVERVIEW.md");
        var description = await File.ReadAllTextAsync(overview, cancellationToken);

        document.Info ??= new OpenApiInfo();
        document.Info.Title = "API COMERCIAL PARA CONSÓRCIO";
        document.Info.Description = description;
    }
}
