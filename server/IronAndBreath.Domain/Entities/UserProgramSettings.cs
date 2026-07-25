namespace IronAndBreath.Domain.Entities;

/// <summary>
/// Single-row settings for the local user's program. Multi-user support would
/// key this by user id later.
/// </summary>
public class UserProgramSettings
{
    public int Id { get; set; }

    /// <summary>Anchor date used to compute the current program week and phase.</summary>
    public DateOnly ProgramStartDate { get; set; }

    public int DaysPerWeekTarget { get; set; } = 4;
}
