using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web_messaging_app.Featuers.Auth.Register;
public static class RegisterEndpoint
{
    public static void MapRegisterEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/register", async (
            [FromBody] RegisterCommand command,
            ISender sender,
            CancellationToken ct) =>
        {
            var response = await sender.Send(command, ct);
            return Results.Created($"/api/users/{response.UserId}", response);
        }).WithName("Register")
        .WithTags("Auth")
        .Produces<RegisterResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status409Conflict)
        .AllowAnonymous();
    }
}
