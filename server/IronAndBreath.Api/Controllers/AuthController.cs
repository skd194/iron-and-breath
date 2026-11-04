using System.Text.RegularExpressions;
using Google.Apis.Auth;
using IronAndBreath.Api.Auth;
using IronAndBreath.Api.Dtos;
using IronAndBreath.Domain.Entities;
using IronAndBreath.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IronAndBreath.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private static readonly Regex EmailPattern =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    private readonly AppDbContext _db;
    private readonly IJwtTokenService _tokens;
    private readonly UserProvisioningService _provisioning;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly GoogleAuthOptions _google;
    private readonly ICurrentUser _me;

    public AuthController(
        AppDbContext db,
        IJwtTokenService tokens,
        UserProvisioningService provisioning,
        IPasswordHasher<User> passwordHasher,
        IOptions<GoogleAuthOptions> google,
        ICurrentUser me)
    {
        _db = db;
        _tokens = tokens;
        _provisioning = provisioning;
        _passwordHasher = passwordHasher;
        _google = google.Value;
        _me = me;
    }

    /// <summary>Which sign-in providers are enabled (read before login by the SPA).</summary>
    [HttpGet("config")]
    [AllowAnonymous]
    public ActionResult<AuthConfigDto> GetConfig()
        => Ok(new AuthConfigDto(_google.Enabled, _google.Enabled ? _google.ClientId : null));

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken ct)
    {
        var email = (request.Email ?? string.Empty).Trim().ToLowerInvariant();
        if (!EmailPattern.IsMatch(email))
        {
            return ValidationProblem("A valid email address is required.");
        }
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            return ValidationProblem("Password must be at least 8 characters.");
        }
        if (await _db.Users.AnyAsync(u => u.Email == email, ct))
        {
            return Conflict(new { message = "An account with that email already exists." });
        }

        var user = new User
        {
            Email = email,
            DisplayName = string.IsNullOrWhiteSpace(request.DisplayName)
                ? email.Split('@')[0]
                : request.DisplayName!.Trim(),
            CreatedAt = DateTimeOffset.UtcNow,
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
        await _provisioning.ProvisionAsync(user, ct);

        return Ok(BuildAuthResponse(user));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        var email = (request.Email ?? string.Empty).Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

        if (user?.PasswordHash is null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password ?? string.Empty);
        if (result == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password!);
            await _db.SaveChangesAsync(ct);
        }

        return Ok(BuildAuthResponse(user));
    }

    [HttpPost("google")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Google(GoogleLoginRequest request, CancellationToken ct)
    {
        if (!_google.Enabled)
        {
            return ValidationProblem("Google sign-in is not configured on this server.");
        }
        if (string.IsNullOrWhiteSpace(request.IdToken))
        {
            return ValidationProblem("Missing Google credential.");
        }

        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _google.ClientId },
            });
        }
        catch (InvalidJwtException)
        {
            return Unauthorized(new { message = "Invalid Google credential." });
        }

        var email = payload.Email?.Trim().ToLowerInvariant() ?? string.Empty;

        // Match on Google subject first, then fall back to email to link accounts.
        var user = await _db.Users.FirstOrDefaultAsync(u => u.GoogleSubject == payload.Subject, ct)
                   ?? await _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

        if (user is null)
        {
            user = new User
            {
                Email = email,
                DisplayName = string.IsNullOrWhiteSpace(payload.Name)
                    ? (email.Contains('@') ? email.Split('@')[0] : "Athlete")
                    : payload.Name,
                GoogleSubject = payload.Subject,
                CreatedAt = DateTimeOffset.UtcNow,
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);
            await _provisioning.ProvisionAsync(user, ct);
        }
        else if (user.GoogleSubject is null)
        {
            user.GoogleSubject = payload.Subject;
            await _db.SaveChangesAsync(ct);
        }

        return Ok(BuildAuthResponse(user));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> Me(CancellationToken ct)
    {
        var user = await _db.Users.FindAsync(new object[] { _me.Id }, ct);
        return user is null ? Unauthorized() : Ok(ToUserDto(user));
    }

    private AuthResponse BuildAuthResponse(User user)
    {
        var (token, expiresAt) = _tokens.Create(user);
        return new AuthResponse(token, expiresAt, ToUserDto(user));
    }

    private static UserDto ToUserDto(User user)
        => new(user.Id, user.Email, user.DisplayName, user.PasswordHash is not null, user.GoogleSubject is not null);
}
