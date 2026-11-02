namespace Apta.WebApi.Transformers;

[ExcludeFromCodeCoverage(Justification = "does not contain any logic that can be tested.")]
public sealed class TagsTransformer : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Metadata", "TAGS.md");
        var content = await File.ReadAllTextAsync(path, cancellationToken);

        /* we use https://xoofx.github.io/markdig/ to make markdown parsing easier */
        var blocks = Markdown.Parse(content);

        var headings = blocks.OfType<HeadingBlock>().Where(heading => heading.Level == 2);
        var separators = blocks.OfType<ThematicBreakBlock>();

        var controllers = document.Paths.Values
            .SelectMany(item => item.Operations!.Values)
            .SelectMany(operation => operation.Tags!)
            .Select(tag => tag.Name!)
            .Distinct()
            .ToDictionary(name => name, StringComparer.OrdinalIgnoreCase);

        document.Tags ??= new HashSet<OpenApiTag>();

        foreach (var (heading, separator) in headings.Zip(separators))
        {
            var name = heading.Inline?.FirstChild?.ToString() ?? string.Empty;
            var description = content[(heading.Span.End + 1)..separator.Span.Start].Trim();

            var existing = document.Tags.FirstOrDefault(tag => string.Equals(tag.Name, name, StringComparison.OrdinalIgnoreCase));
            if (existing is not null)
            {
                existing.Description = description;
                continue;
            }

            document.Tags.Add(new OpenApiTag
            {
                Name = name,
                Description = description
            });
        }
    }
}
