namespace Apta.Application.Transport;

public interface IFileStream
{
    public String FileName { get; init; }
    public Stream Stream { get; init; }
}
