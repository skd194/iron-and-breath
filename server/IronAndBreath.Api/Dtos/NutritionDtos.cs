namespace IronAndBreath.Api.Dtos;

public record NutritionProfileDto(
    decimal WeightKg,
    int ProteinTargetG,
    int CalorieTarget,
    string DietStyle,
    string? Notes);

public record UpdateNutritionProfileRequest(
    decimal WeightKg,
    int ProteinTargetG,
    int CalorieTarget,
    string DietStyle,
    string? Notes);

/// <summary>A planned meal. Slot is a string ("Breakfast"/"Lunch"/"Snack"/"Dinner").</summary>
public record MealDto(
    int Id,
    string Slot,
    string? TimeHint,
    string Items,
    int ProteinG,
    int SortOrder);

/// <summary>DayOfWeek is numeric (0=Sun..6=Sat) to match the workout schedule.</summary>
public record DietDayDto(
    int Id,
    DayOfWeek DayOfWeek,
    bool IsFastingDay,
    string? Notes,
    int TotalProteinG,
    IReadOnlyList<MealDto> Meals);

public record NutritionDto(
    NutritionProfileDto Profile,
    IReadOnlyList<DietDayDto> Days);

public record UpdateDietDayRequest(bool IsFastingDay, string? Notes);

public record UpsertMealRequest(string Slot, string? TimeHint, string Items, int ProteinG);
