using FluentAssertions;
using IronAndBreath.Domain.Entities;
using IronAndBreath.Domain.Progression;

namespace IronAndBreath.Tests;

public class ProgressionCalculatorTests
{
    private static readonly DateOnly Start = new(2026, 1, 1);

    private static List<ProgramProgressionPhase> Phases() =>
    [
        new() { Id = 1, PhaseNumber = 1, WeekStart = 1, WeekEnd = 4 },
        new() { Id = 2, PhaseNumber = 2, WeekStart = 5, WeekEnd = 8 },
        new() { Id = 3, PhaseNumber = 3, WeekStart = 9, WeekEnd = 12 },
    ];

    [Theory]
    [InlineData(0, 1)]   // start date itself is week 1
    [InlineData(6, 1)]   // last day of week 1
    [InlineData(7, 2)]   // first day of week 2
    [InlineData(27, 4)]  // week 4
    [InlineData(28, 5)]  // first day of week 5
    [InlineData(83, 12)] // last day of week 12
    [InlineData(84, 13)] // past the program
    public void ResolveWeekNumber_maps_elapsed_days_to_weeks(int elapsedDays, int expectedWeek)
    {
        var today = Start.AddDays(elapsedDays);
        ProgressionCalculator.ResolveWeekNumber(Start, today).Should().Be(expectedWeek);
    }

    [Fact]
    public void ResolveWeekNumber_clamps_dates_before_start_to_week_one()
    {
        ProgressionCalculator.ResolveWeekNumber(Start, Start.AddDays(-5)).Should().Be(1);
    }

    [Theory]
    [InlineData(0, 1)]   // week 1  -> phase 1
    [InlineData(27, 1)]  // week 4  -> phase 1
    [InlineData(28, 2)]  // week 5  -> phase 2
    [InlineData(55, 2)]  // week 8  -> phase 2
    [InlineData(56, 3)]  // week 9  -> phase 3
    [InlineData(83, 3)]  // week 12 -> phase 3
    public void ResolvePhase_maps_weeks_to_phases(int elapsedDays, int expectedPhase)
    {
        var today = Start.AddDays(elapsedDays);
        ProgressionCalculator.ResolvePhase(Start, today, Phases()).PhaseNumber.Should().Be(expectedPhase);
    }

    [Fact]
    public void ResolvePhase_clamps_past_program_to_last_phase()
    {
        var today = Start.AddDays(200); // well past week 12
        ProgressionCalculator.ResolvePhase(Start, today, Phases()).PhaseNumber.Should().Be(3);
    }

    [Fact]
    public void ResolvePhase_clamps_before_start_to_first_phase()
    {
        ProgressionCalculator.ResolvePhase(Start, Start.AddDays(-10), Phases()).PhaseNumber.Should().Be(1);
    }

    [Fact]
    public void ResolvePhase_throws_when_no_phases_configured()
    {
        var act = () => ProgressionCalculator.ResolvePhase(Start, Start, []);
        act.Should().Throw<InvalidOperationException>();
    }
}
