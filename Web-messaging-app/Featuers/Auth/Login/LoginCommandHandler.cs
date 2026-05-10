using MediatR;
using Microsoft.EntityFrameworkCore;
using Web_messaging_app.Infrastructure.Auth.JWT;
using Web_messaging_app.Infrastructure.Persistence.PostgreSql;
namespace Web_messaging_app.Featuers.Auth.Login;

public class LoginCommandHandler(AppDbContext _dbContext, IJwtService _jwtService) : IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(a => a.Email == request.Email, cancellationToken);
        if(user is null)
        {
            throw new UnauthorizedAccessException("Invalid Email or Password");
        }
        var pass = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if(!pass)
        {
            throw new UnauthorizedAccessException("Invalid Email or Password");
        }
        var refreshToken = _jwtService.GenerateRefreshToken(user.Id);
        var accessToken = _jwtService.GenerateAccessToken(user);

        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new LoginResponse(user.Id, user.UserName, user.AvatarUrl, accessToken, refreshToken.Token);
    }
}
