namespace IronAndBreath.Domain.Entities;

/// <summary>
/// One day of the weekly meal plan. Null <see cref="UserId"/> marks a seeded
/// template day cloned into each new user's plan (mirrors <see cref="WorkoutDay"/>).
/// </summary>
public class DietDay
{
    public int Id { get; set; }

    /// <summary>Owning user. Null = seeded template cloned at provisioning.</summary>
    public int? UserId { get; set; }
    public User? User { get; set; }

    /// <summary>Weekday this plan applies to.</summary>
    public DayOfWeek DayOfWeek { get; set; }

    /// <summary>
    /// A gentle fasting day (one protein-forward meal + fluids) for gut rest.
    /// Flagged in the UI with guidance rather than enforced.
    /// </summary>
    public bool IsFastingDay { get; set; }

    public string? Notes { get; set; }

    public ICollection<Meal> Meals { get; set; } = new List<Meal>();
}

/// <summary>Which part of the day a meal belongs to.</summary>
public enum MealSlot
{
    Breakfast = 0,
    Lunch = 1,
    Snack = 2,
    Dinner = 3,
}

/// <summary>A single planned meal within a <see cref="DietDay"/>.</summary>
public class Meal
{
    public int Id { get; set; }

    public int DietDayId { get; set; }
    public DietDay? DietDay { get; set; }

    public MealSlot Slot { get; set; }

    /// <summary>When to eat, e.g. "7:30 AM" or "post-workout".</summary>
    public string? TimeHint { get; set; }

    /// <summary>The foods, free-text, e.g. "Overnight oats + Greek yogurt + peanut butter".</summary>
    public string Items { get; set; } = string.Empty;

    /// <summary>Rough protein contribution in grams, for the daily total.</summary>
    public int ProteinG { get; set; }

    public int SortOrder { get; set; }
}
