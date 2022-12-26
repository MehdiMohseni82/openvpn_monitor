using DotnetTalk.OpenVpn.Monitor.Telnet.Connector.Models;

namespace DotnetTalk.OpenVpn.Monitor.Telnet.Connector.Interfaces;

public interface IOpenVpnClient
{
    delegate void OnByteCountReceivedHandler(object sender, ByteCount e);
    event OnByteCountReceivedHandler OnByteCountReceived;

    delegate void OnConnectionReceivedHandler(object sender, Connection e);
    public event OnConnectionReceivedHandler OnConnectionReceived;

    Task Connect();

    Task TurnOnNotification(int interval);

    Task StartListening();
}