using MediatR;
namespace Web_messaging_app.Featuers.Auth.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<RefreshTokenResponse>;
