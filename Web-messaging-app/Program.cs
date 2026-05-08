
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Web_messaging_app.Infrastructure.Persistence.MongoDb;
using Web_messaging_app.Infrastructure.Persistence.PostgreSql;

namespace Web_messaging_app
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddSingleton<IMongoClient>(sp =>
            new MongoClient(builder.Configuration["MongoDB:ConnectionString"]));

            builder.Services.AddSingleton<MongoDbContext>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var mongoContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
                await MongoDbIndexInitializer.InitializeAsync(mongoContext);
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.Run();
        }
    }
}
