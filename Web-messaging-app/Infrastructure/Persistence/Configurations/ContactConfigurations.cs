using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web_messaging_app.Domain.Models.UserModule;

namespace Web_messaging_app.Infrastructure.Persistence.Configurations;

public class ContactConfigurations : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("Contacts");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(a => a.Name)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(a => a.IsMuted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(a => a.IsBlocked)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(a => a.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.HasIndex(c => new { c.OwnerId, c.ContactUserId })
            .IsUnique();

        builder.HasOne(a => a.Owner)
            .WithMany(a => a.Contacts)
            .HasForeignKey(a => a.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasOne(a => a.ContactUser)
            .WithMany()
            .HasForeignKey(a => a.ContactUserId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
