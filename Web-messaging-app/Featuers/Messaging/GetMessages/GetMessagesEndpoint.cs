using MediatR;

namespace Web_messaging_app.Featuers.Messaging.GetMessages;

public static class GetMessagesEndpoint
{
    public static void MapGetMessagesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/conversations/{conversationId:guid}/messages", async (
            Guid conversationId,
            int page,
            int pageSize,
            ISender sender,
            CancellationToken ct) =>
        {
            var response = await sender.Send(
                new GetMessagesQuery(conversationId, page, pageSize), ct);
            return Results.Ok(response);
        })
        .WithName("GetMessages")
        .WithTags("Messaging")
        .Produces<GetMessagesResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();
    }
}
