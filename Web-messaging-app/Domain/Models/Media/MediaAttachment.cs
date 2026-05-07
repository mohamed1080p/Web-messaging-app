using Web_messaging_app.Domain.Models.UserModule;

namespace Web_messaging_app.Domain.Models.Media;
public class MediaAttachment
{
    public Guid Id { get; set; }
    public Guid UploadedByUserId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string StorageUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public MediaProcessingStatus ProcessingStatus { get; set; }
    public DateTime CreatedAt { get; set; }

    ///////////////////////////////////////////////////////////////////
    public User UploadedBy { get; set; } = null!;
}
