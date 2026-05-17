using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Web_messaging_app.Domain.Models.Messages;

public class Message
{
    [BsonId]
    public string Id { get; set; } = string.Empty;
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid ConversationId { get; set; }
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid SenderId { get; set; }
    public MessageType Type { get; set; }
    public string? Text { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
