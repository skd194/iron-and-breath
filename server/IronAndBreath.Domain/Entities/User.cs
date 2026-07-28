namespace IronAndBreath.Domain.Entities;

/// <summary>
/// An account. A user may sign in with a password, with Google, or both linked
/// to the same email. Each user owns an isolated copy of the program (workout
/// days + exercises), their own settings, and their own logged sessions.
/// </summary>
public class User
{
    public int Id { get; set; }

    /// <summary>Stored lower-cased; unique across all users.</summary>
    public string Email { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    /// <summary>PBKDF2 hash (ASP.NET Core PasswordHasher). Null for Google-only accounts.</summary>
    public string? PasswordHash { get; set; }

    /// <summary>Google account subject ("sub") claim. Null until Google is linked.</summary>
    public string? GoogleSubject { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<WorkoutDay> WorkoutDays { get; set; } = new List<WorkoutDay>();
    public ICollection<WorkoutSession> Sessions { get; set; } = new List<WorkoutSession>();
}
