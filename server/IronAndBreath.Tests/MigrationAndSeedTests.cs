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
    public void Migrations_apply_and_seed_the_four_day_split()
    {
        _db.WorkoutDays.Count().Should().Be(4);
        _db.Exercises.Count().Should().Be(24);
        _db.ProgramProgressionPhases.Count().Should().Be(3);
    }

    [Fact]
    public void Each_day_has_six_exercises_in_sort_order()
    {
        foreach (var day in _db.WorkoutDays.Include(d => d.Exercises))
        {
            day.Exercises.Should().HaveCount(6);
            day.Exercises.Select(e => e.SortOrder).Should().BeInAscendingOrder();
        }
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
