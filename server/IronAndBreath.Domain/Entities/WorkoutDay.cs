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

    public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
    public ICollection<WorkoutSession> Sessions { get; set; } = new List<WorkoutSession>();
}
