using MediatR;

namespace Web_messaging_app.Featuers.Conversations.GetConversations;

public static class GetConversationsEndpoint
{
    public static void MapGetConversationsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/conversations", async (
            ISender sender,
            CancellationToken ct) =>
        {
            var response = await sender.Send(new GetConversationsQuery(), ct);
            return Results.Ok(response);
        })
        .WithName("GetConversations")
        .WithTags("Conversations")
        .Produces<List<ConversationListDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();
    }
}
