using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web_messaging_app.Featuers.Auth.Logout;
public static class LogoutEndpoint
{
    public static void MapLogoutEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/auth/logout", async ([FromBody] LogoutCommand Command, ISender sender, CancellationToken ct) =>
        {
            await sender.Send(Command, ct);
            return Results.NoContent();
        }).WithName("Logout")
        .WithTags("Auth")
        .Produces(StatusCodes.Status204NoContent)
        .RequireAuthorization();
    }
}
