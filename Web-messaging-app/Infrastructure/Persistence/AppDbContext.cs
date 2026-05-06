using Microsoft.EntityFrameworkCore;
using Web_messaging_app.Domain.Models.UserModule;

namespace Web_messaging_app.Infrastructure.Persistence;
public class AppDbContext:DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
    {
        
    }
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

}
