using DotnetTalk.OpenVpn.Monitor.Writer.MongoDb.Models;
using MongoDB.Driver;

namespace DotnetTalk.OpenVpn.Monitor.Writer.MongoDb.Interfaces
{
    public interface IOpenVpnMonitorDbContext
    {
        IMongoCollection<Client> Clients { get; }
    }
}
