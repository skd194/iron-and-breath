using FluentAssertions;
using IronAndBreath.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace IronAndBreath.Tests;

/// <summary>
/// Applies the real EF Core migrations against an in-memory SQLite database and
/// checks the baked-in seed. This guards both the schema and the HasData seed.
/// </summary>
public class MigrationAndSeedTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _db;

    public MigrationAndSeedTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _db = new AppDbContext(options);
        _db.Database.Migrate();
    }

    [Fact]
    public void Migrations_apply_and_seed_the_five_day_split()
    {
        _db.WorkoutDays.Count().Should().Be(5);
        _db.Exercises.Count().Should().Be(38);
        _db.ProgramProgressionPhases.Count().Should().Be(3);
    }

    [Fact]
    public void Each_day_is_scheduled_with_exercises_in_sort_order()
    {
        foreach (var day in _db.WorkoutDays.Include(d => d.Exercises))
        {
            day.Exercises.Should().NotBeEmpty();
            day.Exercises.Select(e => e.SortOrder).Should().BeInAscendingOrder();
            day.ScheduledDayOfWeek.Should().NotBeNull();
        }

        // The user trains Sat/Sun/Mon/Wed/Thu; Tue + Fri are rest (no day assigned).
        _db.WorkoutDays.Select(d => d.ScheduledDayOfWeek)
            .Should().BeEquivalentTo(new DayOfWeek?[]
            {
                DayOfWeek.Saturday, DayOfWeek.Sunday, DayOfWeek.Monday,
                DayOfWeek.Wednesday, DayOfWeek.Thursday,
            });
    }

    [Fact]
    public void Seeded_exercises_carry_coaching_and_tempo_metadata()
    {
        // Dynamic lifts get breathing cues, muscles and a structured tempo; holds
        // get a breathing note and no tempo.
        var bench = _db.Exercises.Single(e => e.Name == "Flat Barbell Bench Press");
        bench.PrimaryMuscles.Should().Contain("Chest");
        bench.BreathingConcentric.Should().Be("Exhale");
        bench.BreathingEccentric.Should().Be("Inhale");
        bench.TempoConcentricSeconds.Should().NotBeNull();
        bench.TempoEccentricSeconds.Should().NotBeNull();

        var plank = _db.Exercises.Single(e => e.Name == "Plank Hold");
        plank.BreathingConcentric.Should().BeNull();
        plank.BreathingNotes.Should().NotBeNullOrEmpty();
        plank.TempoConcentricSeconds.Should().BeNull();
    }

    [Fact]
    public void Seeds_reference_catalogs_and_templates()
    {
        // Muscle reference catalog (global), equipment + diet templates (UserId == null).
        _db.Muscles.Should().NotBeEmpty();
        _db.Muscles.Should().Contain(m => m.Name == "Chest" && m.Function != "");

        _db.EquipmentItems.Where(e => e.UserId == null).Should().NotBeEmpty();
        _db.DietDays.Where(d => d.UserId == null).Should().HaveCount(7);
        _db.DietDays.Where(d => d.UserId == null && d.IsFastingDay).Should().ContainSingle();
        _db.Meals.Should().NotBeEmpty();
    }

    [Fact]
    public void Phases_cover_weeks_one_through_twelve_without_gaps()
    {
        var phases = _db.ProgramProgressionPhases.OrderBy(p => p.WeekStart).ToList();
        phases.Select(p => p.WeekStart).Should().Equal(1, 5, 9);
        phases.Select(p => p.WeekEnd).Should().Equal(4, 8, 12);
        phases.Last().SetDelta.Should().Be(1); // phase 3 adds a set
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }
}
