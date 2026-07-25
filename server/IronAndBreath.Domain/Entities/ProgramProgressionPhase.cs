namespace IronAndBreath.Domain.Entities;

/// <summary>
/// One of the three 4-week phases of the 3-month program. All tunable
/// progression parameters live here (data, not hardcoded in the UI).
/// </summary>
public class ProgramProgressionPhase
{
    public int Id { get; set; }

    public int PhaseNumber { get; set; }

    /// <summary>First program week (1-based, inclusive) this phase applies to.</summary>
    public int WeekStart { get; set; }

    /// <summary>Last program week (1-based, inclusive) this phase applies to.</summary>
    public int WeekEnd { get; set; }

    public string RepsHintText { get; set; } = string.Empty;

    /// <summary>Added to each exercise's BaseSets for this phase (e.g. +1 in phase 3).</summary>
    public int SetDelta { get; set; }

    /// <summary>Duration of a single work step, in seconds.</summary>
    public int RestWorkSeconds { get; set; }

    public int RestBetweenSetsSeconds { get; set; }

    public int RestBetweenExercisesSeconds { get; set; }

    public string? Notes { get; set; }
}
