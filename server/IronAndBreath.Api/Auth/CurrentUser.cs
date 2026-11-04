using System.Security.Claims;

namespace IronAndBreath.Api.Auth;

/// <summary>Resolves the authenticated user's id from the request principal.</summary>
public interface ICurrentUser
{
    bool IsAuthenticated { get; }

    /// <summary>The authenticated user's id. Throws if the request is unauthenticated.</summary>
    int Id { get; }
}

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUser(IHttpContextAccessor accessor) => _accessor = accessor;

    public bool IsAuthenticated =>
        _accessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

    public int Id
    {
        get
        {
            var raw = _accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(raw, out var id))
            {
                return id;
            }

            throw new InvalidOperationException("No authenticated user on the current request.");
        }
    }
}
