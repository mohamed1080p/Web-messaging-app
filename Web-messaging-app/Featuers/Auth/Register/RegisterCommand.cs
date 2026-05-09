using MediatR;
namespace Web_messaging_app.Featuers.Auth.Register;

public record RegisterCommand(string Email,
    string Password,
    string Username,
    string PhoneNumber) : IRequest<RegisterResponse>;
