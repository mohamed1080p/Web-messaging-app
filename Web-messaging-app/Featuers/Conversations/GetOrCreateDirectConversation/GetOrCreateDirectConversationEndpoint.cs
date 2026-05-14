using MediatR;

namespace Web_messaging_app.Featuers.Conversations.GetOrCreateDirectConversation;

public static class GetOrCreateDirectConversationEndpoint
{
    public static void MapGetOrCreateDirectConversationEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/conversations/direct/{contactUserId:guid}", async (
            Guid contactUserId,
            ISender sender,
            CancellationToken ct) =>
        {
            var response = await sender.Send(
                new GetOrCreateDirectConversationCommand(contactUserId), ct);
            return Results.Ok(response);
        })
        .WithName("GetOrCreateDirectConversation")
        .WithTags("Conversations")
        .Produces<ConversationResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();
    }
}
