using MediatR;
using Microsoft.EntityFrameworkCore;
using Web_messaging_app.Infrastructure.Auth.JWT;
using Web_messaging_app.Infrastructure.Persistence.PostgreSql;

namespace Web_messaging_app.Featuers.Auth.RefreshToken;

public class RefreshTokenCommandHandler(AppDbContext _dbContext, IJwtService _jwtService)
    : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var existingToken = await _dbContext.RefreshTokens
            .Include(r => r.User)
            .SingleOrDefaultAsync(r => r.Token == request.RefreshToken, ct);

        if (existingToken is null)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        if (!existingToken.IsActive)
        {
            _dbContext.RefreshTokens.Remove(existingToken);
            await _dbContext.SaveChangesAsync(ct);
            throw new UnauthorizedAccessException("Refresh token has expired. Please log in again.");
        }

        var newAccessToken = _jwtService.GenerateAccessToken(existingToken.User);
        var newRefreshToken = _jwtService.GenerateRefreshToken(existingToken.User.Id);

        _dbContext.RefreshTokens.Remove(existingToken);
        _dbContext.RefreshTokens.Add(newRefreshToken);
        await _dbContext.SaveChangesAsync(ct);

        return new RefreshTokenResponse(
            newAccessToken,
            newRefreshToken.Token
        );
    }
}
