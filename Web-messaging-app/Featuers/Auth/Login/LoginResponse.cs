namespace Web_messaging_app.Featuers.Auth.Login;

public record LoginResponse(Guid UserId, string Username, string? AvatarUrl, string AccessToken, string RefreshToken);
