using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web_messaging_app.Featuers.Messaging.SendMessage;

public static class SendMessageEndpoint
{
    public static void MapSendMessageEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/conversations/{conversationId:guid}/messages", async (
            Guid conversationId,
            [FromBody] SendMessageRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new SendMessageCommand(conversationId, request.Text);
            var response = await sender.Send(command, ct);
            return Results.Created($"/api/conversations/{conversationId}/messages/{response.MessageId}", response);
        })
        .WithName("SendMessage")
        .WithTags("Messaging")
        .Produces<SendMessageResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();
    }
}

// Separate request body since ConversationId comes from the URL
public record SendMessageRequest(string? Text);
