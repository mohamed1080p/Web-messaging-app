using MediatR;

namespace Web_messaging_app.Featuers.Contacts.GetContacts;
public static class GetContactsEndpoint
{
    public static void MapGetContactsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/contacts", async (
            ISender sender,
            CancellationToken ct) =>
        {
            var response = await sender.Send(new GetContactsQuery(), ct);
            return Results.Ok(response);
        })
        .WithName("GetContacts")
        .WithTags("Contacts")
        .Produces<List<ContactDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();
    }
}
