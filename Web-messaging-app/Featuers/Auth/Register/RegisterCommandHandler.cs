using BCrypt.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Web_messaging_app.Domain.Models.UserModule;
using Web_messaging_app.Infrastructure.Auth.JWT;
using Web_messaging_app.Infrastructure.Persistence.PostgreSql;

namespace Web_messaging_app.Featuers.Auth.Register;

public class RegisterCommandHandler(AppDbContext _dbContext, IJwtService _jwtService) : IRequestHandler<RegisterCommand, RegisterResponse>
{
    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var EmailExists = await _dbContext.Users.AnyAsync(a => a.Email == request.Email, cancellationToken);
        var PhoneNumberExists = await _dbContext.Users.AnyAsync(a => a.PhoneNumber == request.PhoneNumber, cancellationToken);
        if (EmailExists || PhoneNumberExists)
        {
            throw new Exception("Email or PhoneNumber already exists");
        }

        var PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User()
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = PasswordHash,
            UserName = request.Username,
            CreatedAt = DateTime.UtcNow,
            PhoneNumber=request.PhoneNumber
        };

        var AccessToken = _jwtService.GenerateAccessToken(user);
        var RefreshToken = _jwtService.GenerateRefreshToken(user.Id);

        _dbContext.Users.Add(user);
        _dbContext.RefreshTokens.Add(RefreshToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new RegisterResponse(
            user.Id,
            user.Email,
            user.UserName,
            RefreshToken.Token,
            AccessToken
            );

    }
}
