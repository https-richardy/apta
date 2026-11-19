namespace Apta.WebApi.Binders;

public sealed class BlobModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        return context.Metadata.ModelType == typeof(Blob)
            ? new BlobModelBinder()
            : (IModelBinder?)null;
    }
}
