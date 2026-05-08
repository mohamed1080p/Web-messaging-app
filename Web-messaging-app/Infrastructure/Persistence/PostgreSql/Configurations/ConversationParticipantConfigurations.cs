using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web_messaging_app.Domain.Models.Conversations;

namespace Web_messaging_app.Infrastructure.Persistence.PostgreSql.Configurations;

public class ConversationParticipantConfigurations : IEntityTypeConfiguration<ConversationParticipant>
{
    public void Configure(EntityTypeBuilder<ConversationParticipant> builder)
    {
        builder.ToTable("ConversationParticipants");

        builder.HasKey(cp => cp.Id);

        builder.Property(cp => cp.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(cp => cp.Role)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(cp => cp.JoinedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.Property(cp => cp.MutedUntil)
            .IsRequired(false);

        builder.Property(cp => cp.LastReadAt)
            .IsRequired(false);

        builder.HasIndex(cp => new { cp.ConversationId, cp.UserId }).IsUnique();

        builder.HasOne(cp => cp.Conversation)
            .WithMany(c => c.Participants)
            .HasForeignKey(cp => cp.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cp => cp.User)
            .WithMany(u => u.ConversationParticipants)
            .HasForeignKey(cp => cp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
