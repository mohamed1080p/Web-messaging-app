using MediatR;
namespace Web_messaging_app.Featuers.Auth.Login;

public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;