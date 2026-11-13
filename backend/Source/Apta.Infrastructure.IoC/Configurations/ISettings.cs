namespace Apta.Infrastructure.IoC.Configurations;

public interface ISettings
{
    public AuthenticationSettings Authentication { get; init; }
    public DatabaseSettings Database { get; init; }
    public ObservabilitySettings Observability { get; init; }
}
