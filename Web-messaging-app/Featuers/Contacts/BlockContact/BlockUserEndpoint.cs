using MediatR;

namespace Web_messaging_app.Featuers.Contacts.BlockContact;

public static class BlockUserEndpoint
{
    public static void MapBlockUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/contacts/block/{userId:guid}", async (
            Guid userId,
            ISender sender,
            CancellationToken ct) =>
        {
            await sender.Send(new BlockUserCommand(userId), ct);
            return Results.NoContent();
        })
        .WithName("BlockUser")
        .WithTags("Contacts")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();
    }
}
