/* chore: this is a no-op comment to force the pr-validation pipeline to run */

namespace Apta.WebApi;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        builder.Services.AddInfrastructure(configuration);
        builder.Services.AddWebInfrastructure();

        builder.Configuration.AddEnvironmentVariables();

        var app = builder.Build();

        app.MapOpenApi();

        app.UseMiddlewares();
        app.UseReference();

        await app.RunAsync();
    }
}
