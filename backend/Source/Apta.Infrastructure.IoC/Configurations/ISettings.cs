namespace Apta.Infrastructure.IoC.Configurations;

public interface ISettings
{
    public DatabaseSettings Database { get; init; }
}
