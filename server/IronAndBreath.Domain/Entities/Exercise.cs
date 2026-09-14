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

    // ---- Coaching metadata (drives the interactive workout + rest screens) ----
    // Kept on the exercise for now; promoted to a shared exercise catalog in a
    // later phase. All nullable/additive so existing rows remain valid.

    /// <summary>Breathing cue for the concentric (exertion) phase, e.g. "Exhale".</summary>
    public string? BreathingConcentric { get; set; }

    /// <summary>Breathing cue for the eccentric (return) phase, e.g. "Inhale".</summary>
    public string? BreathingEccentric { get; set; }

    /// <summary>Free-text breathing guidance for holds/edge cases, e.g. "Breathe steadily".</summary>
    public string? BreathingNotes { get; set; }

    /// <summary>Primary muscles worked, stored comma-separated (e.g. "Chest,Triceps").</summary>
    public string? PrimaryMuscles { get; set; }

    /// <summary>Secondary muscles worked, stored comma-separated.</summary>
    public string? SecondaryMuscles { get; set; }

    /// <summary>Movement tempo, e.g. "2-0-2" or "slow eccentric".</summary>
    public string? Tempo { get; set; }

    /// <summary>Key benefits, shown on the rest/detail screens.</summary>
    public string? Benefits { get; set; }

    /// <summary>Common mistakes to avoid.</summary>
    public string? CommonMistakes { get; set; }

    /// <summary>Safety/technique guidance.</summary>
    public string? SafetyTips { get; set; }

    /// <summary>
    /// Asset key for the movement animation slot (resolved on the client).
    /// Null falls back to the instructional video/placeholder.
    /// </summary>
    public string? AnimationRef { get; set; }
}
