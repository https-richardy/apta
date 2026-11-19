namespace Apta.WebApi.Transformers;

[ExcludeFromCodeCoverage(Justification = "does not contain any logic that can be tested.")]
public sealed class BlobSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (context.JsonTypeInfo.Type != typeof(Blob))
        {
            return Task.CompletedTask;
        }

        /* a string typed as "binary" tells openapi tooling (scalar) to render a file upload input instead of a text field */
        /* https://swagger.io/docs/specification/describing-request-body/file-upload/ */
        schema.Format = "binary";

        schema.ContentMediaType = MediaTypeNames.Application.Octet;
        schema.Type = JsonSchemaType.String;
        schema.Properties?.Clear();

        return Task.CompletedTask;
    }
}
