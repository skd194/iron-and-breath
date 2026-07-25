using IronAndBreath.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IronAndBreath.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the DbContext against whichever provider the "Database" section
    /// selects. This is the single place the provider is chosen.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>()
                      ?? new DatabaseOptions();

        services.AddDbContext<AppDbContext>(db => ConfigureProvider(db, options));

        return services;
    }

    /// <summary>
    /// Shared provider-selection logic, reused by the design-time factory so the
    /// tooling and the app always agree on how the provider is wired.
    /// </summary>
    public static void ConfigureProvider(DbContextOptionsBuilder builder, DatabaseOptions options)
    {
        const string migrationsAssembly = "IronAndBreath.Infrastructure";

        switch (options.Provider)
        {
            case DatabaseProvider.Postgres:
                builder.UseNpgsql(options.ConnectionString, npgsql =>
                    npgsql.MigrationsAssembly(migrationsAssembly));
                break;

            case DatabaseProvider.Sqlite:
            default:
                builder.UseSqlite(options.ConnectionString, sqlite =>
                    sqlite.MigrationsAssembly(migrationsAssembly));
                break;
        }
    }
}
