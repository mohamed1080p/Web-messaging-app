using MediatR;
using Microsoft.EntityFrameworkCore;
using Web_messaging_app.Infrastructure.Persistence.PostgreSql;

namespace Web_messaging_app.Featuers.Auth.Logout;

public class LogoutCommandHandler(AppDbContext _dbContext) : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var token = await _dbContext.RefreshTokens.FirstOrDefaultAsync(a => a.Token == request.RefreshToken, cancellationToken);
        if(token is null)
        {
            return;
        }
        _dbContext.RefreshTokens.Remove(token);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
