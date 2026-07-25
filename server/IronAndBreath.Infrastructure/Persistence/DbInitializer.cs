using IronAndBreath.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IronAndBreath.Infrastructure.Persistence;

public static class DbInitializer
{
    /// <summary>
    /// Applies pending migrations and ensures the single settings row exists.
    /// Static reference data (days/exercises/phases) arrives via HasData in the
    /// migration itself; the settings row is created here so its start date can
    /// default to "today" rather than being baked into a migration.
    /// </summary>
    public static async Task MigrateAndSeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        await db.Database.MigrateAsync(ct);

        if (!await db.UserProgramSettings.AnyAsync(ct))
        {
            db.UserProgramSettings.Add(new UserProgramSettings
            {
                ProgramStartDate = DateOnly.FromDateTime(DateTime.Today),
                DaysPerWeekTarget = 4
            });
            await db.SaveChangesAsync(ct);
        }
    }
}
