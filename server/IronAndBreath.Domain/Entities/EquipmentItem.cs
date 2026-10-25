namespace IronAndBreath.Domain.Entities;

/// <summary>Broad category of a piece of equipment, for grouping in the UI.</summary>
public enum EquipmentCategory
{
    Bench = 0,
    Barbell = 1,
    Dumbbell = 2,
    Plates = 3,
    Rack = 4,
    Resistance = 5,
    Cardio = 6,
    Bodyweight = 7,
    Accessory = 8,
    Other = 9,
}

/// <summary>Whether the user already owns an item or plans to acquire it.</summary>
public enum EquipmentStatus
{
    Owned = 0,
    Planned = 1,
}

/// <summary>
/// A piece of gym equipment the user owns (or plans to). Null <see cref="UserId"/>
/// marks a seeded template item cloned into each new user's inventory, mirroring
/// <see cref="WorkoutDay"/>. Fed to the AI coach as available-equipment context.
/// </summary>
public class EquipmentItem
{
    public int Id { get; set; }

    /// <summary>Owning user. Null = seeded template cloned at provisioning.</summary>
    public int? UserId { get; set; }
    public User? User { get; set; }

    public string Name { get; set; } = string.Empty;

    public EquipmentCategory Category { get; set; }

    /// <summary>Free-text spec, e.g. "Incline/Decline/Flat, 250kg, resistance ropes".</summary>
    public string? Specs { get; set; }

    public EquipmentStatus Status { get; set; } = EquipmentStatus.Owned;

    public int SortOrder { get; set; }
}
