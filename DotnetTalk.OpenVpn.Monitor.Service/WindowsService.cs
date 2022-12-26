using DotnetTalk.OpenVpn.Monitor.Telnet.Connector.Interfaces;
using DotnetTalk.OpenVpn.Monitor.Telnet.Connector.Models;
using DotnetTalk.OpenVpn.Monitor.Writer.MongoDb.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotnetTalk.OpenVpn.Monitor.Service;

public class WindowsService : IHostedService
{
    private readonly ILogger _logger;
    private readonly IOpenVpnClient _openVpnClient;
    private readonly IListener _listener;

    public WindowsService(
        ILogger<WindowsService> logger,
        IListener listener,
        IOpenVpnClient openVpnClient)
    {
        _logger = logger;
        _openVpnClient = openVpnClient;
        _listener = listener;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _openVpnClient.Connect();
        await _openVpnClient.TurnOnNotification(1);

        _openVpnClient.OnByteCountReceived += _openVpnClient_OnByteCountReceived;
        _openVpnClient.OnConnectionReceived += _openVpnClient_OnConnectionReceived;

        Task.Run(_openVpnClient.StartListening, cancellationToken);
        Task.Run(_listener.Start, cancellationToken);

        _logger.LogError($"Starting windows service.");
    }

    private void _openVpnClient_OnConnectionReceived(object sender, Connection e)
    {
        _logger.LogInformation($"{e}");
    }

    private void _openVpnClient_OnByteCountReceived(object sender, ByteCount e)
    {
        _logger.LogInformation($"{e}");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Stopping windows service.");
        await Task.Delay(20, cancellationToken);
    }
}