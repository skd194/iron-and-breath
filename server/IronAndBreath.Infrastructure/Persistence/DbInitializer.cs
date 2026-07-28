using Microsoft.EntityFrameworkCore;

namespace IronAndBreath.Infrastructure.Persistence;

public static class DbInitializer
{
    /// <summary>
    /// Applies pending migrations. Static reference data (template days/exercises
    /// and the progression phases) arrives via HasData in the migration itself.
    /// Per-user settings and the user's editable program copy are created during
    /// account provisioning, not here.
    /// </summary>
    public static async Task MigrateAndSeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        await db.Database.MigrateAsync(ct);
    }
}
