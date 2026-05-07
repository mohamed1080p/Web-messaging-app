using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web_messaging_app.Domain.Models.Conversations;

namespace Web_messaging_app.Infrastructure.Persistence.Configurations;

public class ConversationConfigurations : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("Conversations");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(c => c.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(c => c.Title)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(c => c.Description)
            .HasMaxLength(200)
            .IsRequired(false);

        builder.Property(c => c.AvatarUrl)
            .HasMaxLength(512)
            .IsRequired(false);

        builder.Property(c => c.LastMessagePreview)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.Property(c => c.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.HasOne(c => c.CreatedBy)
            .WithMany()
            .HasForeignKey(c => c.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
