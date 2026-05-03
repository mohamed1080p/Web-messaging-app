using Microsoft.EntityFrameworkCore;

namespace Web_messaging_app.Infrastructure.Persistence;
public class AppDbContext:DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
    {
        
    }

}
