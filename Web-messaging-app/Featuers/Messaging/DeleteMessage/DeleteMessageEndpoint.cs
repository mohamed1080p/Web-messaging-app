using MediatR;

namespace Web_messaging_app.Featuers.Messaging.DeleteMessage;

public static class DeleteMessageEndpoint
{
    public static void MapDeleteMessageEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/conversations/{conversationId:guid}/messages/{messageId}", async (
            Guid conversationId,
            string messageId,
            ISender sender,
            CancellationToken ct) =>
        {
            await sender.Send(new DeleteMessageCommand(messageId, conversationId), ct);
            return Results.NoContent();
        })
        .WithName("DeleteMessage")
        .WithTags("Messaging")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();
    }
}
