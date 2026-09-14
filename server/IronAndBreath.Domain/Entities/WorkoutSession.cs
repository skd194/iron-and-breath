namespace IronAndBreath.Domain.Entities;

/// <summary>
/// A logged workout. CompletedAt is null when the session was abandoned.
/// </summary>
public class WorkoutSession
{
    public int Id { get; set; }

    /// <summary>Owning user.</summary>
    public int UserId { get; set; }
    public User? User { get; set; }

    /// <summary>
    /// The program day performed. Null for ad-hoc manual sessions (e.g. a workout
    /// done away from the app with exercises not in the user's program).
    /// </summary>
    public int? WorkoutDayId { get; set; }
    public WorkoutDay? WorkoutDay { get; set; }

    /// <summary>How this session was recorded (guided/manual/imported).</summary>
    public WorkoutSource Source { get; set; } = WorkoutSource.Guided;

    /// <summary>Optional free-text notes for the whole session.</summary>
    public string? Notes { get; set; }

    /// <summary>Optional session-level perceived difficulty / RPE (1-10).</summary>
    public int? PerceivedDifficulty { get; set; }

    /// <summary>Calendar date the session was performed (timezone-safe, no time component).</summary>
    public DateOnly Date { get; set; }

    /// <summary>Program phase (1-3) computed at the time the session was completed.</summary>
    public int PhaseNumberAtCompletion { get; set; }

    public DateTimeOffset StartedAt { get; set; }

    /// <summary>Null if the session was abandoned rather than finished.</summary>
    public DateTimeOffset? CompletedAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<SessionSetLog> SetLogs { get; set; } = new List<SessionSetLog>();
}
