using FluentAssertions;
using IronAndBreath.Domain.Entities;
using IronAndBreath.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace IronAndBreath.Tests;

/// <summary>
/// Exercises the 1B logging schema against real migrations: per-set logs on a
/// guided session, and ad-hoc manual sessions with no program day / no linked
/// exercise.
/// </summary>
public class SessionLoggingTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _db;

    public SessionLoggingTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;
        _db = new AppDbContext(options);
        _db.Database.Migrate();
    }

    private User AddUser(string email = "a@example.com")
    {
        var user = new User { Email = email, DisplayName = "Tester", CreatedAt = DateTimeOffset.UtcNow };
        _db.Users.Add(user);
        _db.SaveChanges();
        return user;
    }

    [Fact]
    public void Guided_session_persists_set_logs()
    {
        var user = AddUser();
        var day = new WorkoutDay { UserId = user.Id, Name = "Push", Focus = "", SortOrder = 1 };
        var ex = new Exercise { Name = "Bench Press", RepsDisplay = "", BaseSets = 3 };
        day.Exercises.Add(ex);
        _db.WorkoutDays.Add(day);
        _db.SaveChanges();

        var session = new WorkoutSession
        {
            UserId = user.Id,
            WorkoutDayId = day.Id,
            Date = new DateOnly(2024, 1, 1),
            Source = WorkoutSource.Guided,
            StartedAt = DateTimeOffset.UtcNow,
            CompletedAt = DateTimeOffset.UtcNow,
            CreatedAt = DateTimeOffset.UtcNow,
        };
        session.SetLogs.Add(new SessionSetLog
        {
            ExerciseId = ex.Id,
            ExerciseName = ex.Name,
            SetNumber = 1,
            RepsCompleted = 10,
            WeightKg = 40.5m,
            Rpe = 8,
        });
        _db.WorkoutSessions.Add(session);
        _db.SaveChanges();

        var loaded = _db.WorkoutSessions.AsNoTracking().Include(s => s.SetLogs).Single();
        loaded.Source.Should().Be(WorkoutSource.Guided);
        loaded.WorkoutDayId.Should().Be(day.Id);
        loaded.SetLogs.Should().HaveCount(1);
        loaded.SetLogs.First().WeightKg.Should().Be(40.5m);
        loaded.SetLogs.First().Rpe.Should().Be(8);
    }

    [Fact]
    public void Manual_ad_hoc_session_allows_null_day_and_unlinked_exercise()
    {
        var user = AddUser("b@example.com");

        var session = new WorkoutSession
        {
            UserId = user.Id,
            WorkoutDayId = null, // ad-hoc: no program day
            Date = new DateOnly(2024, 2, 2),
            Source = WorkoutSource.Manual,
            Notes = "Left phone at home",
            PerceivedDifficulty = 7,
            StartedAt = DateTimeOffset.UtcNow,
            CompletedAt = DateTimeOffset.UtcNow,
            CreatedAt = DateTimeOffset.UtcNow,
        };
        session.SetLogs.Add(new SessionSetLog
        {
            ExerciseId = null, // not in the program
            ExerciseName = "Cable Fly",
            SetNumber = 1,
            RepsCompleted = 15,
        });
        _db.WorkoutSessions.Add(session);
        _db.SaveChanges();

        var loaded = _db.WorkoutSessions.AsNoTracking().Include(s => s.SetLogs).Single(s => s.Id == session.Id);
        loaded.WorkoutDayId.Should().BeNull();
        loaded.Source.Should().Be(WorkoutSource.Manual);
        loaded.PerceivedDifficulty.Should().Be(7);
        var log = loaded.SetLogs.Single();
        log.ExerciseId.Should().BeNull();
        log.ExerciseName.Should().Be("Cable Fly");
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }
}
