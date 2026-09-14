namespace IronAndBreath.Domain.Entities;

/// <summary>
/// Optional per-set record for future rep/weight tracking. Weight uses
/// <see cref="decimal"/> with explicit precision for cross-provider safety.
/// </summary>
public class SessionSetLog
{
    public int Id { get; set; }

    public int WorkoutSessionId { get; set; }
    public WorkoutSession? WorkoutSession { get; set; }

    /// <summary>
    /// The program exercise this set belongs to. Null for ad-hoc manual entries
    /// whose exercise isn't in the program (see <see cref="ExerciseName"/>).
    /// </summary>
    public int? ExerciseId { get; set; }
    public Exercise? Exercise { get; set; }

    /// <summary>
    /// Display name for the exercise. Populated for ad-hoc sets, and as a durable
    /// label so history survives the linked exercise being renamed or deleted.
    /// </summary>
    public string? ExerciseName { get; set; }

    public int SetNumber { get; set; }

    public int? RepsCompleted { get; set; }

    public decimal? WeightKg { get; set; }

    /// <summary>Optional per-set perceived exertion (RPE 1-10).</summary>
    public int? Rpe { get; set; }

    /// <summary>Optional per-set note.</summary>
    public string? Notes { get; set; }
}
