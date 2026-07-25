namespace IronAndBreath.Domain.Entities;

/// <summary>
/// A single movement within a <see cref="WorkoutDay"/>.
/// </summary>
public class Exercise
{
    public int Id { get; set; }

    public int WorkoutDayId { get; set; }
    public WorkoutDay? WorkoutDay { get; set; }

    public string Name { get; set; } = string.Empty;

    /// <summary>Lower bound of the target rep range, null when not rep-based (e.g. timed holds).</summary>
    public int? TargetRepsLow { get; set; }

    /// <summary>Upper bound of the target rep range, null when not rep-based.</summary>
    public int? TargetRepsHigh { get; set; }

    /// <summary>Human-readable target for edge cases like "to near-failure" or "45-60 sec".</summary>
    public string RepsDisplay { get; set; } = string.Empty;

    /// <summary>Short form/technique cue shown alongside the video.</summary>
    public string? Cue { get; set; }

    public int SortOrder { get; set; }

    /// <summary>Default number of work sets before the progression phase's SetDelta is applied.</summary>
    public int BaseSets { get; set; }

    public int? VideoId { get; set; }
    public ExerciseVideo? Video { get; set; }
}
