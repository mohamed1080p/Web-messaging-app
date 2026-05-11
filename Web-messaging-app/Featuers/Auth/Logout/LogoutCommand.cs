using MediatR;
namespace Web_messaging_app.Featuers.Auth.Logout;

public record LogoutCommand(string RefreshToken) : IRequest;
