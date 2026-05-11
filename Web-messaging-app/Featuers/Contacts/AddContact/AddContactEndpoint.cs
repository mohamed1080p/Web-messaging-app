using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web_messaging_app.Featuers.Contacts.AddContact;
public static class AddContactEndpoint
{
    public static void MapAddContactEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/contacts", async (
            [FromBody] AddContactCommand command,
            ISender sender,
            CancellationToken ct) =>
        {
            var response = await sender.Send(command, ct);
            return Results.Created($"/api/contacts/{response.ContactId}", response);
        })
        .WithName("AddContact")
        .WithTags("Contacts")
        .Produces<AddContactResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();
    }
}
