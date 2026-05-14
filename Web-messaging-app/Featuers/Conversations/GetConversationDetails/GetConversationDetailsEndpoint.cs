using MediatR;
namespace Web_messaging_app.Featuers.Conversations.GetConversationDetails;

public static class GetConversationDetailsEndpoint
{
    public static void MapGetConversationDetailsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/conversations/{conversationId:guid}", async (
            Guid conversationId,
            ISender sender,
            CancellationToken ct) =>
        {
            var response = await sender.Send(
                new GetConversationDetailsQuery(conversationId), ct);
            return Results.Ok(response);
        })
        .WithName("GetConversationDetails")
        .WithTags("Conversations")
        .Produces<ConversationDetailsDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization();
    }
}
