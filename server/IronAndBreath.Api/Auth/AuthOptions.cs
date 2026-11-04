namespace IronAndBreath.Api.Auth;

/// <summary>JWT signing/validation parameters, bound from the "Jwt" section.</summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "iron-and-breath";
    public string Audience { get; set; } = "iron-and-breath";

    /// <summary>HMAC signing key. Must be at least 32 bytes. Supplied via config/env; never hardcoded.</summary>
    public string Key { get; set; } = string.Empty;

    public int ExpiryMinutes { get; set; } = 60 * 24 * 30; // 30 days
}

/// <summary>Google sign-in config, bound from the "Google" section.</summary>
public class GoogleAuthOptions
{
    public const string SectionName = "Google";

    /// <summary>OAuth 2.0 Web Client ID. Empty disables Google sign-in.</summary>
    public string ClientId { get; set; } = string.Empty;

    public bool Enabled => !string.IsNullOrWhiteSpace(ClientId);
}
