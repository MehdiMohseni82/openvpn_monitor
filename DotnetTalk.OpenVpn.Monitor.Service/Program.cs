using DotnetTalk.OpenVpn.Monitor.Common;
using DotnetTalk.OpenVpn.Monitor.Service;
using DotnetTalk.OpenVpn.Monitor.Telnet.Connector.Interfaces;
using DotnetTalk.OpenVpn.Monitor.Writer.MongoDb;
using DotnetTalk.OpenVpn.Monitor.Writer.MongoDb.Interfaces;
using DotnetTalk.OpenVpn.Telnet.Connector;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using YamlDotNet.Serialization;

var yaml = File.ReadAllText("configuration.yaml");
var deserializer = new DeserializerBuilder().Build();
var configuration = deserializer.Deserialize<OpenVpnMonitorConfiguration>(yaml);

CreateHostBuilder(args ?? Array.Empty<string>(), configuration)
    .Build()
    .Run();

static IHostBuilder CreateHostBuilder(string[] args, OpenVpnMonitorConfiguration configuration)
{
    var host = Host.CreateDefaultBuilder(args)
        .UseWindowsService()
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config
                .AddEnvironmentVariables()
                .AddCommandLine(args);
        })
        .ConfigureServices((hostContext, services) =>
        {
            services.AddOptions();
            services.AddSingleton(configuration);
            services.AddSingleton<WindowsService>();
            services.AddTransient<IOpenVpnClient, OpenVpnClient>();
            services.AddTransient<IListener, Listener>();
            services.AddTransient<IOpenVpnMonitorDbContext, OpenVpnMonitorDbContext>();
            services.AddTransient<IHostedService, WindowsService>();
        })
        .ConfigureLogging((hostContext, logging) =>
        {
            logging.ClearProviders();
            logging.AddNLog();
        });

    return host;
}