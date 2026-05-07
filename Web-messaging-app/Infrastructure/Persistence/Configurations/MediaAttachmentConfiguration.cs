using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web_messaging_app.Domain.Models.Media;

namespace Web_messaging_app.Infrastructure.Persistence.Configurations;
public class MediaAttachmentConfiguration : IEntityTypeConfiguration<MediaAttachment>
{
    public void Configure(EntityTypeBuilder<MediaAttachment> builder)
    {
        builder.ToTable("MediaAttachments");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(m => m.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(m => m.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.SizeBytes)
            .IsRequired();

        builder.Property(m => m.StorageUrl)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(m => m.ThumbnailUrl)
            .HasMaxLength(512)
            .IsRequired(false);

        builder.Property(m => m.ProcessingStatus)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(m => m.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.HasOne(m => m.UploadedBy)
            .WithMany()
            .HasForeignKey(m => m.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
