namespace Apta.WebApi.Transformers;

[ExcludeFromCodeCoverage(Justification = "does not contain any logic that can be tested.")]
public sealed class SecurityTransformer(ISettings settings) : IOpenApiDocumentTransformer
{
    private readonly OpenApiOAuthFlow _authorizationCode = new()
    {
        AuthorizationUrl = settings.Authentication.Authority
            .AppendPathSegment("protocol")
            .AppendPathSegment("openid-connect")
            .AppendPathSegment("auth")
            .ToUri(),

        TokenUrl = settings.Authentication.Authority
            .AppendPathSegment("protocol")
            .AppendPathSegment("openid-connect")
            .AppendPathSegment("token")
            .ToUri(),

        Scopes = new Dictionary<string, string>
        {
            ["openid"] = "open-id connect scope",
            ["profile"] = "user profile information",
            ["email"] = "user email information"
        }
    };

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes[SecuritySchemeType.OAuth2.ToString()] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = _authorizationCode
            }
        };

        return Task.CompletedTask;
    }
}
