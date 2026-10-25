namespace IronAndBreath.Domain.Entities;

/// <summary>
/// A reference entry describing a muscle: what it does, its everyday use, and how
/// to avoid injuring it. Global (no owner), seeded for every muscle name used in
/// the program. Surfaced on the rest screen so the user can study the muscles the
/// upcoming exercise works. <see cref="Name"/> matches the strings stored in
/// <see cref="Exercise.PrimaryMuscles"/>/<see cref="Exercise.SecondaryMuscles"/>.
/// </summary>
public class Muscle
{
    public int Id { get; set; }

    /// <summary>Display name, unique; matches the comma-separated muscle strings on exercises.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Coarse grouping, e.g. "Chest", "Back", "Legs", "Core", "Shoulders", "Arms".</summary>
    public string Group { get; set; } = string.Empty;

    /// <summary>What the muscle does and what it's used for in daily life / training.</summary>
    public string Function { get; set; } = string.Empty;

    /// <summary>How to train it safely / what commonly injures it.</summary>
    public string? InjuryNotes { get; set; }
}
