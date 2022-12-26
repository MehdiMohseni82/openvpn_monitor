using DotnetTalk.OpenVpn.Monitor.Common;
using DotnetTalk.OpenVpn.Monitor.Writer.MongoDb.Interfaces;
using DotnetTalk.OpenVpn.Monitor.Writer.MongoDb.Models;
using MongoDB.Driver;

namespace DotnetTalk.OpenVpn.Monitor.Writer.MongoDb;

public class OpenVpnMonitorDbContext : IOpenVpnMonitorDbContext
{
    private readonly IMongoDatabase _mongoDatabase;

    public OpenVpnMonitorDbContext(OpenVpnMonitorConfiguration configuration)
    {
        var mongoClient = new MongoClient(configuration.MongoDbWriter.ConnectionString);
        _mongoDatabase = mongoClient.GetDatabase(configuration.MongoDbWriter.Database);
    }

    public IMongoCollection<Client> Clients => _mongoDatabase.GetCollection<Client>("Clients");
}