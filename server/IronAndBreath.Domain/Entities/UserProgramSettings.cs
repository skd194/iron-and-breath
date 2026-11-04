namespace IronAndBreath.Domain.Entities;

/// <summary>
/// Per-user settings for the program. One row per user (unique on UserId).
/// </summary>
public class UserProgramSettings
{
    public int Id { get; set; }

    /// <summary>Owning user.</summary>
    public int UserId { get; set; }
    public User? User { get; set; }

    /// <summary>Anchor date used to compute the current program week and phase.</summary>
    public DateOnly ProgramStartDate { get; set; }

    public int DaysPerWeekTarget { get; set; } = 4;
}
