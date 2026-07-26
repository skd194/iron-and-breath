using FluentAssertions;
using IronAndBreath.Domain.Stats;

namespace IronAndBreath.Tests;

public class StatsCalculatorTests
{
    // 2024-01-01 is a Monday, which keeps the week-anchor maths easy to read.
    private static readonly DateOnly Mon0101 = new(2024, 1, 1);

    private static CompletedSessionRecord S(DateOnly date, int dayId = 1) => new(date, dayId);

    [Theory]
    [InlineData(2024, 1, 1, 2024, 1, 1)]  // Monday -> itself
    [InlineData(2024, 1, 7, 2024, 1, 1)]  // Sunday -> previous Monday
    [InlineData(2024, 1, 8, 2024, 1, 8)]  // next Monday -> itself
    [InlineData(2024, 1, 10, 2024, 1, 8)] // Wednesday -> that week's Monday
    public void MondayOf_anchors_to_monday(int y, int m, int d, int ey, int em, int ed)
    {
        StatsCalculator.MondayOf(new DateOnly(y, m, d)).Should().Be(new DateOnly(ey, em, ed));
    }

    [Fact]
    public void Compute_returns_zeros_when_no_sessions()
    {
        var result = StatsCalculator.Compute([], Mon0101.AddDays(9), Mon0101, 4);

        result.TotalSessions.Should().Be(0);
        result.SessionsThisWeek.Should().Be(0);
        result.CurrentWeeklyStreak.Should().Be(0);
        result.AverageSessionsPerWeek.Should().Be(0);
        result.MonthlyTarget.Should().Be(16);
        result.PerDayCompletedCounts.Should().BeEmpty();
    }

    [Fact]
    public void Streak_counts_consecutive_weeks_including_current()
    {
        var today = new DateOnly(2024, 1, 10); // week of Jan 8
        var sessions = new[]
        {
            S(new DateOnly(2024, 1, 9)),   // current week (Jan 8)
            S(new DateOnly(2024, 1, 3)),   // prev week (Jan 1)
            S(new DateOnly(2023, 12, 27)), // week before (Dec 25)
        };

        StatsCalculator.Compute(sessions, today, Mon0101, 4).CurrentWeeklyStreak.Should().Be(3);
    }

    [Fact]
    public void Streak_has_grace_period_for_empty_current_week()
    {
        var today = new DateOnly(2024, 1, 10); // current week (Jan 8) is empty
        var sessions = new[]
        {
            S(new DateOnly(2024, 1, 3)),   // prev week
            S(new DateOnly(2023, 12, 27)), // week before
        };

        StatsCalculator.Compute(sessions, today, Mon0101, 4).CurrentWeeklyStreak.Should().Be(2);
    }

    [Fact]
    public void Streak_breaks_on_a_missing_week()
    {
        var today = new DateOnly(2024, 1, 10);
        var sessions = new[]
        {
            S(new DateOnly(2024, 1, 9)),   // current week
            // Jan 1 week missing -> breaks
            S(new DateOnly(2023, 12, 27)),
        };

        StatsCalculator.Compute(sessions, today, Mon0101, 4).CurrentWeeklyStreak.Should().Be(1);
    }

    [Fact]
    public void Counts_week_month_and_per_day()
    {
        var today = new DateOnly(2024, 1, 10);
        var sessions = new[]
        {
            S(new DateOnly(2024, 1, 8), 1),  // this week, this month
            S(new DateOnly(2024, 1, 9), 1),  // this week, this month
            S(new DateOnly(2024, 1, 2), 2),  // last week, this month
            S(new DateOnly(2023, 12, 20), 3) // last month
        };

        var r = StatsCalculator.Compute(sessions, today, Mon0101, 4);

        r.TotalSessions.Should().Be(4);
        r.SessionsThisWeek.Should().Be(2);
        r.SessionsThisMonth.Should().Be(3);
        r.PerDayCompletedCounts[1].Should().Be(2);
        r.PerDayCompletedCounts[2].Should().Be(1);
        r.PerDayCompletedCounts[3].Should().Be(1);
    }

    [Fact]
    public void Average_per_week_divides_total_by_weeks_elapsed()
    {
        // Program started Jan 1, "today" Jan 10 -> 9 elapsed days -> 2 weeks.
        var today = new DateOnly(2024, 1, 10);
        var sessions = new[]
        {
            S(new DateOnly(2024, 1, 2)),
            S(new DateOnly(2024, 1, 8)),
            S(new DateOnly(2024, 1, 9)),
        };

        StatsCalculator.Compute(sessions, today, Mon0101, 4).AverageSessionsPerWeek.Should().Be(1.5);
    }
}
