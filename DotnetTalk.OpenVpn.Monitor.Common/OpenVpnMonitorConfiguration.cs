using DotnetTalk.OpenVpn.Monitor.Common.Configuration;

namespace DotnetTalk.OpenVpn.Monitor.Common;
public class OpenVpnMonitorConfiguration
{
    public OpenVpnConnection OpenVpnConnection { get; set; }

    public MongoDbWriter MongoDbWriter { get; set; }

    public RabbitMqConfiguration RabbitMqConfiguration { get; set; }
}