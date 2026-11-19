namespace Apta.WebApi.Binders;

public sealed class BlobModelBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var request = bindingContext.HttpContext.Request;
        if (!request.HasFormContentType)
        {
            return;
        }

        var form = await request.ReadFormAsync(bindingContext.HttpContext.RequestAborted);
        if (form.Files.Count == 0)
        {
            return;
        }

        var file = form.Files[0];
        var memory = new MemoryStream();

        await file.CopyToAsync(memory, bindingContext.HttpContext.RequestAborted);

        memory.Position = 0;
        bindingContext.Result = ModelBindingResult.Success(new Blob
        {
            FileName = file.FileName,
            Stream = memory
        });
    }
}
