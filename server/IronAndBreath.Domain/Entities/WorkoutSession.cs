namespace IronAndBreath.Domain.Entities;

/// <summary>
/// A logged workout. CompletedAt is null when the session was abandoned.
/// </summary>
public class WorkoutSession
{
    public int Id { get; set; }

    public int WorkoutDayId { get; set; }
    public WorkoutDay? WorkoutDay { get; set; }

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
