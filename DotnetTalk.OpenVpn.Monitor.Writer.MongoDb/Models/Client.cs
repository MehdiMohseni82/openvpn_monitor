using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DotnetTalk.OpenVpn.Monitor.Writer.MongoDb.Models;

public class Client
{
    [BsonId]
    public ObjectId Id { get; set; }

    public Client()
    {
        Id = ObjectId.GenerateNewId();
    }
}