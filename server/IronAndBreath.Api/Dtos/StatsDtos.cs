namespace IronAndBreath.Api.Dtos;

public record StatsSummaryDto(
    int TotalSessions,
    int SessionsThisWeek,
    int SessionsThisMonth,
    int MonthlyTarget,
    int CurrentWeeklyStreak,
    double AverageSessionsPerWeek,
    IReadOnlyDictionary<int, int> PerDayCompletedCounts);
