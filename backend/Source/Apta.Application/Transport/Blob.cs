namespace Apta.Application.Transport;

public sealed record Blob
{
    public String FileName { get; init; } = default!;
    public Stream Stream { get; init; } = default!;
}
