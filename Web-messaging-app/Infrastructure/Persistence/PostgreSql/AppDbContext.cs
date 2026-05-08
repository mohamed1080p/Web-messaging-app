using Microsoft.EntityFrameworkCore;
using Web_messaging_app.Domain.Models.Contacts;
using Web_messaging_app.Domain.Models.Conversations;
using Web_messaging_app.Domain.Models.Media;
using Web_messaging_app.Domain.Models.UserModule;

namespace Web_messaging_app.Infrastructure.Persistence.PostgreSql;
public class AppDbContext:DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
    {
        
    }
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<UserBlock> UserBlocks { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<ConversationParticipant> conversationParticipants { get; set; }
    public DbSet<MediaAttachment> MediaAttachments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

}
