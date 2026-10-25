using IronAndBreath.Domain.Entities;
using IronAndBreath.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IronAndBreath.Api.Auth;

/// <summary>
/// Gives a brand-new user their own editable copy of everything seeded as a
/// template (UserId == null): the workout program (days + exercises), the
/// equipment inventory, and the weekly meal plan. Also creates their settings and
/// nutrition-profile rows. Shared reference data (videos, the muscle catalog) is
/// referenced, not copied.
/// </summary>
public class UserProvisioningService
{
    private readonly AppDbContext _db;

    public UserProvisioningService(AppDbContext db) => _db = db;

    /// <summary>Requires <paramref name="user"/> to already be persisted (has an Id).</summary>
    public async Task ProvisionAsync(User user, CancellationToken ct = default)
    {
        var templateDays = await _db.WorkoutDays
            .AsNoTracking()
            .Where(d => d.UserId == null)
            .Include(d => d.Exercises)
            .OrderBy(d => d.SortOrder)
            .ToListAsync(ct);
        foreach (var template in templateDays)
        {
            _db.WorkoutDays.Add(CloneDay(template, user.Id));
        }

        var templateEquipment = await _db.EquipmentItems
            .AsNoTracking()
            .Where(e => e.UserId == null)
            .OrderBy(e => e.SortOrder)
            .ToListAsync(ct);
        foreach (var template in templateEquipment)
        {
            _db.EquipmentItems.Add(CloneEquipment(template, user.Id));
        }

        var templateDiet = await _db.DietDays
            .AsNoTracking()
            .Where(d => d.UserId == null)
            .Include(d => d.Meals)
            .ToListAsync(ct);
        foreach (var template in templateDiet)
        {
            _db.DietDays.Add(CloneDietDay(template, user.Id));
        }

        _db.UserProgramSettings.Add(new UserProgramSettings
        {
            UserId = user.Id,
            ProgramStartDate = DateOnly.FromDateTime(DateTime.Today),
            DaysPerWeekTarget = 5,
        });

        _db.NutritionProfiles.Add(DefaultNutritionProfile(user.Id));

        await _db.SaveChangesAsync(ct);
    }

    /// <summary>Default nutrition targets for a new account (editable afterwards).</summary>
    public static NutritionProfile DefaultNutritionProfile(int userId) => new()
    {
        UserId = userId,
        WeightKg = 75m,
        ProteinTargetG = 135,
        CalorieTarget = 2600,
        DietStyle = "Vegetarian · Kerala (South Indian)",
        Notes = "No paneer; limited cheese. Protein from soya, gram/chickpea, peanut, yogurt, egg, oats, pulses.",
    };

    /// <summary>Deep-clones a template workout day (with its exercises) into a user-owned day.</summary>
    public static WorkoutDay CloneDay(WorkoutDay template, int userId)
    {
        var day = new WorkoutDay
        {
            UserId = userId,
            Name = template.Name,
            Focus = template.Focus,
            SortOrder = template.SortOrder,
            ScheduledDayOfWeek = template.ScheduledDayOfWeek,
        };

        foreach (var ex in template.Exercises.OrderBy(e => e.SortOrder))
        {
            day.Exercises.Add(new Exercise
            {
                Name = ex.Name,
                TargetRepsLow = ex.TargetRepsLow,
                TargetRepsHigh = ex.TargetRepsHigh,
                RepsDisplay = ex.RepsDisplay,
                Cue = ex.Cue,
                SortOrder = ex.SortOrder,
                BaseSets = ex.BaseSets,
                VideoId = ex.VideoId,
                // Coaching metadata carries into the user's editable copy.
                BreathingConcentric = ex.BreathingConcentric,
                BreathingEccentric = ex.BreathingEccentric,
                BreathingNotes = ex.BreathingNotes,
                PrimaryMuscles = ex.PrimaryMuscles,
                SecondaryMuscles = ex.SecondaryMuscles,
                Tempo = ex.Tempo,
                TempoConcentricSeconds = ex.TempoConcentricSeconds,
                TempoEccentricSeconds = ex.TempoEccentricSeconds,
                TempoPauseSeconds = ex.TempoPauseSeconds,
                Benefits = ex.Benefits,
                CommonMistakes = ex.CommonMistakes,
                SafetyTips = ex.SafetyTips,
                AnimationRef = ex.AnimationRef,
            });
        }

        return day;
    }

    private static EquipmentItem CloneEquipment(EquipmentItem template, int userId) => new()
    {
        UserId = userId,
        Name = template.Name,
        Category = template.Category,
        Specs = template.Specs,
        Status = template.Status,
        SortOrder = template.SortOrder,
    };

    private static DietDay CloneDietDay(DietDay template, int userId)
    {
        var day = new DietDay
        {
            UserId = userId,
            DayOfWeek = template.DayOfWeek,
            IsFastingDay = template.IsFastingDay,
            Notes = template.Notes,
        };
        foreach (var meal in template.Meals.OrderBy(m => m.SortOrder))
        {
            day.Meals.Add(new Meal
            {
                Slot = meal.Slot,
                TimeHint = meal.TimeHint,
                Items = meal.Items,
                ProteinG = meal.ProteinG,
                SortOrder = meal.SortOrder,
            });
        }
        return day;
    }
}
