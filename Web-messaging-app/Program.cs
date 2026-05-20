using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;
using Web_messaging_app.Featuers.Auth.Login;
using Web_messaging_app.Featuers.Auth.Logout;
using Web_messaging_app.Featuers.Auth.RefreshToken;
using Web_messaging_app.Featuers.Auth.Register;
using Web_messaging_app.Featuers.Contacts.AddContact;
using Web_messaging_app.Featuers.Contacts.BlockContact;
using Web_messaging_app.Featuers.Contacts.GetContacts;
using Web_messaging_app.Featuers.Contacts.RemoveContact;
using Web_messaging_app.Featuers.Conversations.CreateGroupConversation;
using Web_messaging_app.Featuers.Conversations.GetConversationDetails;
using Web_messaging_app.Featuers.Conversations.GetConversations;
using Web_messaging_app.Featuers.Conversations.GetOrCreateDirectConversation;
using Web_messaging_app.Featuers.Messaging.DeleteMessage;
using Web_messaging_app.Featuers.Messaging.GetMessages;
using Web_messaging_app.Featuers.Messaging.Hubs;
using Web_messaging_app.Featuers.Messaging.Notifications;
using Web_messaging_app.Featuers.Messaging.SendMessage;
using Web_messaging_app.Infrastructure.Auth.JWT;
using Web_messaging_app.Infrastructure.Persistence.MongoDb;
using Web_messaging_app.Infrastructure.Persistence.PostgreSql;

namespace Web_messaging_app;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddSingleton<IMongoClient>(sp =>
            new MongoClient(builder.Configuration["MongoDB:ConnectionString"]));
        builder.Services.AddSingleton<MongoDbContext>();

        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

        builder.Services.AddScoped<IJwtService, JwtService>();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!))
                };
            });
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddAuthorization();
        builder.Services.AddSignalR();
        builder.Services.AddScoped<INotificationService, NotificationService>();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy
                    .WithOrigins("http://localhost:4200", "null") // "null" allows local HTML files
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials(); // Required for SignalR
            });
        });

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

        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapHub<ChatHub>("/hubs/chat");
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRegisterEndpoint();
        app.MapLoginEndpoint();
        app.MapLogoutEndpoint();
        app.MapRefreshTokenEndpoint();
        app.MapAddContactEndpoint();
        app.MapRemoveContactEndpoint();
        app.MapGetContactsEndpoint();
        app.MapBlockUserEndpoint();
        app.MapGetOrCreateDirectConversationEndpoint();
        app.MapCreateGroupConversationEndpoint();
        app.MapGetConversationsEndpoint();
        app.MapGetConversationDetailsEndpoint();
        app.MapSendMessageEndpoint();
        app.MapGetMessagesEndpoint();
        app.MapDeleteMessageEndpoint();

        app.Run();
    }
}