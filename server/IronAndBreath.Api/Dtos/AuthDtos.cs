namespace IronAndBreath.Api.Dtos;

public record RegisterRequest(string Email, string Password, string? DisplayName);

public record LoginRequest(string Email, string Password);

public record GoogleLoginRequest(string IdToken);

public record UserDto(int Id, string Email, string DisplayName, bool HasPassword, bool GoogleLinked);

public record AuthResponse(string Token, DateTimeOffset ExpiresAt, UserDto User);

/// <summary>Public bootstrap config the SPA reads before login (which providers are on).</summary>
public record AuthConfigDto(bool GoogleEnabled, string? GoogleClientId);
