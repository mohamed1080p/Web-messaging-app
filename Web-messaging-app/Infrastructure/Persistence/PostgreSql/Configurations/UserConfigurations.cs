using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web_messaging_app.Domain.Models.UserModule;

namespace Web_messaging_app.Infrastructure.Persistence.PostgreSql.Configurations;

public class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id).
            HasDefaultValueSql("gen_random_uuid()");

        builder.Property(a => a.UserName)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(a => a.UserName).
            IsUnique();

        builder.Property(a => a.DisplayName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.Email)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.Bio)
            .HasMaxLength(100);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(u => u.AvatarUrl)
            .HasMaxLength(512);

        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.HasIndex(a => a.Email)
            .IsUnique();
    }
}
