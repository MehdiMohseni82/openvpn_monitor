using DotnetTalk.OpenVpn.Monitor.Common;
using DotnetTalk.OpenVpn.Monitor.Telnet.Connector.Interfaces;
using DotnetTalk.OpenVpn.Monitor.Telnet.Connector.Models;
using PrimS.Telnet;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using System.Threading.Channels;
using Microsoft.Extensions.Configuration;

namespace DotnetTalk.OpenVpn.Telnet.Connector
{
    public class OpenVpnClient : IOpenVpnClient
    {
        public event IOpenVpnClient.OnByteCountReceivedHandler OnByteCountReceived;
        public event IOpenVpnClient.OnConnectionReceivedHandler OnConnectionReceived;

        private readonly IModel _channel;
        private readonly OpenVpnMonitorConfiguration _openVpnMonitorConfiguration;
        private Client _client;

        public OpenVpnClient(OpenVpnMonitorConfiguration openVpnMonitorConfiguration)
        {
            _openVpnMonitorConfiguration = openVpnMonitorConfiguration;

            var factory = new ConnectionFactory
            {
                HostName = _openVpnMonitorConfiguration.RabbitMqConfiguration.Host,
                Port = _openVpnMonitorConfiguration.RabbitMqConfiguration.Port,
                UserName = _openVpnMonitorConfiguration.RabbitMqConfiguration.Username,
                Password = _openVpnMonitorConfiguration.RabbitMqConfiguration.Password
            };

            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "openvpn-status", type: ExchangeType.Fanout);
        }

        public async Task Connect()
        {
            _client = new Client(_openVpnMonitorConfiguration.OpenVpnConnection.ServerIp, _openVpnMonitorConfiguration.OpenVpnConnection.ServerPort, CancellationToken.None);
        }

        public async Task TurnOnNotification(int interval)
        {
            await _client.WriteLineAsync("echo on");
            await _client.WriteLineAsync($"bytecount {interval}");
        }

        public async Task StartListening()
        {
            _client.MillisecondReadDelay = 1;
            var toSerialize = "";
            
            await Task.Run(async () =>
            {
                while (true)
                {
                    var message = await _client.ReadAsync(new TimeSpan(0,0,0,1));

                    if (string.IsNullOrEmpty(message))
                    {
                        continue;
                    }

                    if (message.StartsWith(">BYTECOUNT_CLI"))
                    {
                        PublishByteCount(message);
                    }

                    if (message.StartsWith(">CLIENT:ESTABLISHED"))
                    {
                        toSerialize = message;
                    }

                    if (message.Contains(">CLIENT:ENV,END"))
                    {
                        PublishConnection(toSerialize, message);
                        toSerialize = "";
                    }
                }
            });
        }

        private void PublishConnection(string toSerialize, string message)
        {
            toSerialize = $"{toSerialize}{message}";

            var lines = toSerialize.Split(Environment.NewLine);
            var dictionary = lines.Select(line => line.Replace(">CLIENT:ENV,", "").Split("=")).Where(parts => parts.Length > 1)
                .ToDictionary(parts => parts[0], parts => parts[1]);

            var connection = new Connection(dictionary);

            var json = JsonSerializer.Serialize(connection);
            var body = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish("openvpn-status", "", null, body);

            OnConnectionReceived?.Invoke(this, connection);
        }

        private void PublishByteCount(string message)
        {
            message = message.Replace(">BYTECOUNT_CLI:", "");
            var parts = message.Split(",");

            var byteCount = new ByteCount
            {
                ClienId = parts[0],
                ByteIn = int.Parse(parts[1]),
                ByteOut = int.Parse(parts[2])
            };

            var json = JsonSerializer.Serialize(byteCount);
            var body = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish("openvpn-status", "", null, body);

            OnByteCountReceived?.Invoke(this, byteCount);
        }
    }
}