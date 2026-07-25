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

    public int ExerciseId { get; set; }
    public Exercise? Exercise { get; set; }

    public int SetNumber { get; set; }

    public int? RepsCompleted { get; set; }

    public decimal? WeightKg { get; set; }
}
