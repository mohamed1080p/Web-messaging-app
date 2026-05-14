using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web_messaging_app.Featuers.Conversations.GetOrCreateDirectConversation;

namespace Web_messaging_app.Featuers.Conversations.CreateGroupConversation;

public static class CreateGroupConversationEndpoint
{
    public static void MapCreateGroupConversationEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/conversations/group", async (
            [FromBody] CreateGroupConversationCommand command,
            ISender sender,
            CancellationToken ct) =>
        {
            var response = await sender.Send(command, ct);
            return Results.Created($"/api/conversations/{response.ConversationId}", response);
        })
        .WithName("CreateGroupConversation")
        .WithTags("Conversations")
        .Produces<ConversationResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();
    }
}
