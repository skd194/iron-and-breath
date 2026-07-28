namespace IronAndBreath.Domain.Entities;

/// <summary>
/// One day of the 4-day split (e.g. "Upper Body (Push)").
/// </summary>
public class WorkoutDay
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Focus { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    /// <summary>
    /// Owning user. Null marks a seeded <em>template</em> day that is cloned into
    /// each new user's own editable program; it is never shown to users directly.
    /// </summary>
    public int? UserId { get; set; }
    public User? User { get; set; }

    public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
    public ICollection<WorkoutSession> Sessions { get; set; } = new List<WorkoutSession>();
}
