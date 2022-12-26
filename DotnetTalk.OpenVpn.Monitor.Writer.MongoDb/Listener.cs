using System.Text;
using DotnetTalk.OpenVpn.Monitor.Common;
using DotnetTalk.OpenVpn.Monitor.Writer.MongoDb.Interfaces;
using DotnetTalk.OpenVpn.Monitor.Writer.MongoDb.Models;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DotnetTalk.OpenVpn.Monitor.Writer.MongoDb;

public class Listener : IListener
{
    private readonly IOpenVpnMonitorDbContext _context;
    private readonly IModel _channel;
    private readonly EventingBasicConsumer _consumer;
    private readonly OpenVpnMonitorConfiguration _configuration;
    private readonly ILogger<Listener> _logger;

    public Listener(
        OpenVpnMonitorConfiguration configuration,
        IOpenVpnMonitorDbContext context,
        ILogger<Listener> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = _configuration.RabbitMqConfiguration.Host, 
            Port = _configuration.RabbitMqConfiguration.Port, 
            UserName = _configuration.RabbitMqConfiguration.Username, 
            Password = _configuration.RabbitMqConfiguration.Password
        };

        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
        _channel.ExchangeDeclare(exchange: "openvpn-status", type: ExchangeType.Fanout);
        _channel.QueueDeclare("mango-writer-sub");
        _channel.QueueBind(queue: "mango-writer-sub", exchange: "openvpn-status", routingKey: "");

        _consumer = new EventingBasicConsumer(_channel);
        _consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            _logger.LogInformation(message);

            // To-Do
            _context.Clients.InsertOne(new Client());
        };

    }

    public async Task Start()
    {
        _channel.BasicConsume(queue: "mango-writer-sub", autoAck: true, consumer: _consumer);
    }
}