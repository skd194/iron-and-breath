namespace IronAndBreath.Domain.Entities;

/// <summary>
/// Per-user nutrition targets (one row per user, unique on UserId). Seeded at
/// provisioning from the user's bodyweight and goal; editable afterwards.
/// </summary>
public class NutritionProfile
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    /// <summary>Current bodyweight used to derive protein/calorie targets.</summary>
    public decimal WeightKg { get; set; }

    /// <summary>Daily protein target in grams (≈1.6–2.0 g/kg for muscle gain).</summary>
    public int ProteinTargetG { get; set; }

    /// <summary>Daily calorie target (slight surplus for lean gain).</summary>
    public int CalorieTarget { get; set; }

    /// <summary>Dietary style label, e.g. "Vegetarian · Kerala (South Indian)".</summary>
    public string DietStyle { get; set; } = string.Empty;

    /// <summary>Free-text notes (restrictions, preferences, gut sensitivities).</summary>
    public string? Notes { get; set; }
}
