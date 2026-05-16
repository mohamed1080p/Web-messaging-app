using MongoDB.Driver;
using Web_messaging_app.Domain.Models.Messages;
namespace Web_messaging_app.Infrastructure.Persistence.MongoDb;

public static class MongoDbIndexInitializer
{
    public static async Task InitializeAsync(MongoDbContext context)
    {
        var indexKeysDefinition = Builders<Message>.IndexKeys
            .Ascending(m => m.ConversationId)
            .Descending(m => m.CreatedAt);

        await context.Messages.Indexes.CreateOneAsync(
            new CreateIndexModel<Message>(indexKeysDefinition));
    }
}
