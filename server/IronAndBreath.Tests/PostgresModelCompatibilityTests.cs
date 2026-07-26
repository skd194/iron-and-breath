using FluentAssertions;
using IronAndBreath.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IronAndBreath.Tests;

/// <summary>
/// Offline portability check: asks EF to generate the full PostgreSQL schema
/// script from the model (no database connection required). This proves the
/// entire model + HasData seed translates through the Npgsql provider — the
/// DateOnly, DateTimeOffset, decimal-precision, index and seed-data paths all
/// get exercised — so the SQLite -> Postgres swap will hold up.
/// </summary>
public class PostgresModelCompatibilityTests
{
    [Fact]
    public void Model_and_seed_translate_cleanly_to_postgres()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Database=verify;Username=verify;Password=verify")
            .Options;

        using var db = new AppDbContext(options);

        // Offline: builds CREATE TABLE + seed INSERTs via the Npgsql SQL generator.
        var script = db.Database.GenerateCreateScript();

        script.Should().Contain("CREATE TABLE");
        script.Should().Contain("WorkoutDays");
        script.Should().Contain("Exercises");
        script.Should().Contain("ProgramProgressionPhases");
        // Seed data present as inserts.
        script.Should().Contain("Upper Body (Push)");
    }
}
