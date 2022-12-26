namespace DotnetTalk.OpenVpn.Monitor.Telnet.Connector.Models;
public class Connection
{
    public DateTime ConnectedAt { get; set; }

    public string RemoteIp { get; set; }

    public string RemotePort { get; set; }

    public string Name { get; set; }

    public Connection()
    {

    }

    public Connection(Dictionary<string, string> data)
    {
        ConnectedAt = DateTime.Parse(data["time_ascii"]);
        RemoteIp = data["trusted_ip"];
        RemotePort = data["trusted_port"];
        Name = data["common_name"];
    }

    public override string ToString()
    {
        return
            $"{nameof(Connection)} {nameof(Name)}: {Name}, {nameof(ConnectedAt)}: {ConnectedAt}, {nameof(RemoteIp)}: {RemoteIp}, {nameof(RemotePort)}: {RemotePort}";
    }
}
