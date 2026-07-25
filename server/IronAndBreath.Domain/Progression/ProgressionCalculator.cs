using IronAndBreath.Domain.Entities;

namespace IronAndBreath.Domain.Progression;

/// <summary>
/// Pure, DB-free calculation of the current program week and phase. Kept
/// side-effect free so it can be unit tested against off-by-one edge cases.
/// </summary>
public static class ProgressionCalculator
{
    /// <summary>
    /// Program week is 1-based. Week 1 covers program days 0-6 (the start date
    /// itself is week 1). Dates before the start date clamp to week 1.
    /// </summary>
    public static int ResolveWeekNumber(DateOnly programStartDate, DateOnly today)
    {
        var elapsedDays = today.DayNumber - programStartDate.DayNumber;
        if (elapsedDays < 0)
        {
            return 1;
        }

        return (elapsedDays / 7) + 1;
    }

    /// <summary>
    /// Finds the phase whose week range contains <paramref name="today"/>.
    /// Weeks past the final phase clamp to the last phase (program maintenance),
    /// and weeks before the first clamp to the first phase.
    /// </summary>
    public static ProgramProgressionPhase ResolvePhase(
        DateOnly programStartDate,
        DateOnly today,
        IReadOnlyList<ProgramProgressionPhase> phases)
    {
        if (phases is null || phases.Count == 0)
        {
            throw new InvalidOperationException("No progression phases are configured.");
        }

        var ordered = phases.OrderBy(p => p.WeekStart).ToList();
        var week = ResolveWeekNumber(programStartDate, today);

        foreach (var phase in ordered)
        {
            if (week >= phase.WeekStart && week <= phase.WeekEnd)
            {
                return phase;
            }
        }

        // Before the first phase -> first; after the last -> last.
        return week < ordered[0].WeekStart ? ordered[0] : ordered[^1];
    }
}
