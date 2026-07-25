namespace IronAndBreath.Infrastructure.Persistence;

/// <summary>
/// Bound from the "Database" configuration section. Swapping providers is a
/// config change only — no code edits to entities, the DbContext, or queries.
/// </summary>
public class DatabaseOptions
{
    public const string SectionName = "Database";

    public DatabaseProvider Provider { get; set; } = DatabaseProvider.Sqlite;

    public string ConnectionString { get; set; } = string.Empty;
}

public enum DatabaseProvider
{
    Sqlite = 0,
    Postgres = 1
}
