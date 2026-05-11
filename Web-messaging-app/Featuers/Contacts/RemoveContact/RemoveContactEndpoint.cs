using MediatR;

namespace Web_messaging_app.Featuers.Contacts.RemoveContact;
public static class RemoveContactEndpoint
{
    public static void MapRemoveContactEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/contacts/{contactUserId:guid}", async (
            Guid contactUserId,
            ISender sender,
            CancellationToken ct) =>
        {
            await sender.Send(new RemoveContactCommand(contactUserId), ct);
            return Results.NoContent();
        })
        .WithName("RemoveContact")
        .WithTags("Contacts")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();
    }
}
