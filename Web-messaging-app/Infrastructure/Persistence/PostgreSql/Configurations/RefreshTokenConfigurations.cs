using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web_messaging_app.Domain.Models.UserModule;

namespace Web_messaging_app.Infrastructure.Persistence.PostgreSql.Configurations;

public class RefreshTokenConfigurations : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id).
            HasDefaultValueSql("gen_random_uuid()");

        builder.Property(a => a.Token)
            .HasMaxLength(512)
            .IsRequired();

        builder.HasIndex(a => a.Token).
            IsUnique();


        builder.Ignore(a => a.IsActive);

        builder.Property(r => r.ExpiresAt)
           .IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");


        builder.HasIndex(r => r.Token)
            .IsUnique();

        builder.HasOne(r => r.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
