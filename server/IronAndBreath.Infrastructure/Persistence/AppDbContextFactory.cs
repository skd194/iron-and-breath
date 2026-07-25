using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IronAndBreath.Infrastructure.Persistence;

/// <summary>
/// Design-time factory used by the EF Core CLI (`dotnet ef migrations add ...`).
/// Defaults to SQLite; override with environment variables to generate a
/// Postgres migration set at cutover time:
///   DATABASE_PROVIDER=Postgres
///   DATABASE_CONNECTION_STRING="Host=localhost;Database=ironandbreath;Username=postgres;Password=postgres"
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var providerRaw = Environment.GetEnvironmentVariable("DATABASE_PROVIDER");
        var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");

        var provider = Enum.TryParse<DatabaseProvider>(providerRaw, ignoreCase: true, out var parsed)
            ? parsed
            : DatabaseProvider.Sqlite;

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = provider == DatabaseProvider.Postgres
                ? "Host=localhost;Database=ironandbreath;Username=postgres;Password=postgres"
                : "Data Source=ironandbreath-design.db";
        }

        var options = new DatabaseOptions { Provider = provider, ConnectionString = connectionString };

        var builder = new DbContextOptionsBuilder<AppDbContext>();
        DependencyInjection.ConfigureProvider(builder, options);

        return new AppDbContext(builder.Options);
    }
}
