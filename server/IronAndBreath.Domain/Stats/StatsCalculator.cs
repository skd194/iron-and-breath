namespace IronAndBreath.Domain.Stats;

/// <summary>Minimal input record for stats — only what the maths needs.
/// <paramref name="WorkoutDayId"/> is null for ad-hoc manual sessions.</summary>
public readonly record struct CompletedSessionRecord(DateOnly Date, int? WorkoutDayId);

public record StatsSummary(
    int TotalSessions,
    int SessionsThisWeek,
    int SessionsThisMonth,
    int MonthlyTarget,
    int CurrentWeeklyStreak,
    double AverageSessionsPerWeek,
    IReadOnlyDictionary<int, int> PerDayCompletedCounts);

/// <summary>
/// Pure stats maths, isolated from EF so the streak/average edge cases can be
/// unit tested directly. Weeks are ISO-style (Monday-anchored).
/// </summary>
public static class StatsCalculator
{
    public static DateOnly MondayOf(DateOnly date)
    {
        // DayOfWeek: Sunday = 0 ... Saturday = 6. Shift so Monday is the anchor.
        int offset = ((int)date.DayOfWeek + 6) % 7;
        return date.AddDays(-offset);
    }

    public static StatsSummary Compute(
        IReadOnlyList<CompletedSessionRecord> completed,
        DateOnly today,
        DateOnly programStartDate,
        int daysPerWeekTarget)
    {
        var total = completed.Count;

        var thisMonday = MondayOf(today);
        var sessionsThisWeek = completed.Count(s => MondayOf(s.Date) == thisMonday);

        var sessionsThisMonth = completed.Count(s => s.Date.Year == today.Year && s.Date.Month == today.Month);

        var monthlyTarget = daysPerWeekTarget * 4;

        var weekAnchors = completed.Select(s => MondayOf(s.Date)).ToHashSet();
        var streak = ComputeStreak(weekAnchors, thisMonday);

        var weeksElapsed = WeeksElapsed(programStartDate, today);
        var avgPerWeek = weeksElapsed == 0 ? 0 : Math.Round((double)total / weeksElapsed, 2);

        // Per-day counts only apply to program days; ad-hoc manual sessions
        // (null day) still count towards totals/streak but not per-day tallies.
        var perDay = completed
            .Where(s => s.WorkoutDayId != null)
            .GroupBy(s => s.WorkoutDayId!.Value)
            .ToDictionary(g => g.Key, g => g.Count());

        return new StatsSummary(
            total,
            sessionsThisWeek,
            sessionsThisMonth,
            monthlyTarget,
            streak,
            avgPerWeek,
            perDay);
    }

    /// <summary>
    /// Consecutive weeks (ending at the current week) that contain at least one
    /// session. An empty current week does not break the streak — it counts back
    /// from the previous week so an in-progress week isn't penalised.
    /// </summary>
    private static int ComputeStreak(HashSet<DateOnly> weekAnchors, DateOnly currentMonday)
    {
        if (weekAnchors.Count == 0)
        {
            return 0;
        }

        var cursor = weekAnchors.Contains(currentMonday)
            ? currentMonday
            : currentMonday.AddDays(-7);

        var streak = 0;
        while (weekAnchors.Contains(cursor))
        {
            streak++;
            cursor = cursor.AddDays(-7);
        }

        return streak;
    }

    private static int WeeksElapsed(DateOnly programStartDate, DateOnly today)
    {
        var elapsedDays = today.DayNumber - programStartDate.DayNumber;
        if (elapsedDays < 0)
        {
            return 1;
        }

        return (elapsedDays / 7) + 1;
    }
}
