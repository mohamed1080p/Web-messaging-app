using MongoDB.Bson.Serialization.Attributes;

namespace Web_messaging_app.Domain.Models.Messages;
public class Message
{
    [BsonId]
    public string Id { get; set; } = string.Empty;
    public Guid ConversationId { get; set; }
    public long Sequence { get; set; }
    public Guid SenderId { get; set; }
    public MessageType Type { get; set; }
    public string? Text { get; set; }
    public List<MessageAttachment> Attachments { get; set; } = [];
    public string? ReplyToMessageId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
