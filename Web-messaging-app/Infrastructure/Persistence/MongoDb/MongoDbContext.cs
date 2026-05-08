using MongoDB.Driver;
using Web_messaging_app.Domain.Models.Messages;

namespace Web_messaging_app.Infrastructure.Persistence.MongoDb;
public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IMongoClient client, IConfiguration configuration)
    {
        _database = client.GetDatabase(configuration["MongoDB:DatabaseName"]);
    }

    public IMongoCollection<Message> Messages => _database.GetCollection<Message>("Messages");
}
