namespace Web_messaging_app.Featuers.Auth.Register;

public record RegisterResponse(Guid UserId,
    string Email,
    string UserName,
    string DisplayName,
    string RefreshToken,
    string AccessToken);
