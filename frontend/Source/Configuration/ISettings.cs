namespace Apta.WebUI.Configuration;

public interface ISettings
{
    public AuthenticationSettings Authentication { get; }
    public GatewaySettings Gateway { get; }
}
