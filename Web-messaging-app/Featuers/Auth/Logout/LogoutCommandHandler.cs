using MediatR;
using Web_messaging_app.Infrastructure.Persistence.PostgreSql;

namespace Web_messaging_app.Featuers.Auth.Logout
{
    public class LogoutCommandHandler(AppDbContext _dbContext) : IRequestHandler<LogoutCommand>
    {
        public Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            
        }
    }
}
