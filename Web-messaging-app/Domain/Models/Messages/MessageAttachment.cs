namespace Web_messaging_app.Domain.Models.Messages;
public class MessageAttachment
{
    public Guid AttachmentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string StorageUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
}
