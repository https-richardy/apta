namespace Apta.WebApi.Transformers;

[ExcludeFromCodeCoverage(Justification = "does not contain any logic that can be tested.")]
public sealed class BlobTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var carriesBlob = context.Description.ActionDescriptor.Parameters
            .Select(parameter => parameter.ParameterType)
            .Any(type => type == typeof(Blob) || type.GetProperties()
            .Any(property => property.PropertyType == typeof(Blob)));

        if (!carriesBlob)
        {
            return Task.CompletedTask;
        }

        operation.Parameters?.Clear();
        operation.RequestBody = new OpenApiRequestBody
        {
            Required = true,
            Content = new Dictionary<string, OpenApiMediaType>
            {
                [MediaTypeNames.Multipart.FormData] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Required = new HashSet<string> { "file" },
                        Type = JsonSchemaType.Object,
                        Properties = new Dictionary<string, IOpenApiSchema>
                        {
                            ["file"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.String,
                                Format = "binary"
                            }
                        }
                    }
                }
            }
        };

        return Task.CompletedTask;
    }
}
