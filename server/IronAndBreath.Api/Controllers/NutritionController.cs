using IronAndBreath.Api.Auth;
using IronAndBreath.Api.Dtos;
using IronAndBreath.Domain.Entities;
using IronAndBreath.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IronAndBreath.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/nutrition")]
public class NutritionController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ICurrentUser _me;

    public NutritionController(AppDbContext db, ICurrentUser me)
    {
        _db = db;
        _me = me;
    }

    /// <summary>The user's nutrition profile + weekly meal plan (both lazily seeded).</summary>
    [HttpGet]
    public async Task<ActionResult<NutritionDto>> Get(CancellationToken ct)
    {
        var profile = await GetOrCreateProfileAsync(ct);
        await EnsureDaysSeededAsync(ct);
        var days = await OwnedDays().AsNoTracking()
            .Include(d => d.Meals)
            .ToListAsync(ct);
        return Ok(new NutritionDto(ToProfileDto(profile), days.OrderBy(WeekIndex).Select(ToDayDto).ToList()));
    }

    [HttpPut("profile")]
    public async Task<ActionResult<NutritionProfileDto>> UpdateProfile(UpdateNutritionProfileRequest request, CancellationToken ct)
    {
        if (request.WeightKg is <= 0 or > 500)
        {
            return ValidationProblem("Weight must be between 0 and 500 kg.");
        }
        var profile = await GetOrCreateProfileAsync(ct);
        profile.WeightKg = request.WeightKg;
        profile.ProteinTargetG = Math.Max(0, request.ProteinTargetG);
        profile.CalorieTarget = Math.Max(0, request.CalorieTarget);
        profile.DietStyle = request.DietStyle?.Trim() ?? string.Empty;
        profile.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();
        await _db.SaveChangesAsync(ct);
        return Ok(ToProfileDto(profile));
    }

    [HttpPut("days/{id:int}")]
    public async Task<ActionResult<DietDayDto>> UpdateDay(int id, UpdateDietDayRequest request, CancellationToken ct)
    {
        var day = await OwnedDays().Include(d => d.Meals).FirstOrDefaultAsync(d => d.Id == id, ct);
        if (day is null)
        {
            return NotFound();
        }
        day.IsFastingDay = request.IsFastingDay;
        day.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();
        await _db.SaveChangesAsync(ct);
        return Ok(ToDayDto(day));
    }

    [HttpPost("days/{dayId:int}/meals")]
    public async Task<ActionResult<DietDayDto>> CreateMeal(int dayId, UpsertMealRequest request, CancellationToken ct)
    {
        var day = await OwnedDays().Include(d => d.Meals).FirstOrDefaultAsync(d => d.Id == dayId, ct);
        if (day is null)
        {
            return NotFound();
        }
        if (string.IsNullOrWhiteSpace(request.Items))
        {
            return ValidationProblem("Meal items are required.");
        }
        var nextOrder = day.Meals.Count == 0 ? 0 : day.Meals.Max(m => m.SortOrder);
        var meal = new Meal { SortOrder = nextOrder + 1 };
        ApplyMeal(meal, request);
        day.Meals.Add(meal);
        await _db.SaveChangesAsync(ct);
        return Ok(ToDayDto(day));
    }

    [HttpPut("days/{dayId:int}/meals/{mealId:int}")]
    public async Task<ActionResult<DietDayDto>> UpdateMeal(int dayId, int mealId, UpsertMealRequest request, CancellationToken ct)
    {
        var day = await OwnedDays().Include(d => d.Meals).FirstOrDefaultAsync(d => d.Id == dayId, ct);
        var meal = day?.Meals.FirstOrDefault(m => m.Id == mealId);
        if (day is null || meal is null)
        {
            return NotFound();
        }
        if (string.IsNullOrWhiteSpace(request.Items))
        {
            return ValidationProblem("Meal items are required.");
        }
        ApplyMeal(meal, request);
        await _db.SaveChangesAsync(ct);
        return Ok(ToDayDto(day));
    }

    [HttpDelete("days/{dayId:int}/meals/{mealId:int}")]
    public async Task<IActionResult> DeleteMeal(int dayId, int mealId, CancellationToken ct)
    {
        var day = await OwnedDays().Include(d => d.Meals).FirstOrDefaultAsync(d => d.Id == dayId, ct);
        var meal = day?.Meals.FirstOrDefault(m => m.Id == mealId);
        if (day is null || meal is null)
        {
            return NotFound();
        }
        _db.Meals.Remove(meal);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    // ---- Helpers ----

    private IQueryable<DietDay> OwnedDays() => _db.DietDays.Where(d => d.UserId == _me.Id);

    // Order the week from Monday so the plan reads Mon..Sun.
    private static int WeekIndex(DietDay d) => ((int)d.DayOfWeek + 6) % 7;

    private async Task<NutritionProfile> GetOrCreateProfileAsync(CancellationToken ct)
    {
        var profile = await _db.NutritionProfiles.FirstOrDefaultAsync(p => p.UserId == _me.Id, ct);
        if (profile is null)
        {
            profile = UserProvisioningService.DefaultNutritionProfile(_me.Id);
            _db.NutritionProfiles.Add(profile);
            await _db.SaveChangesAsync(ct);
        }
        return profile;
    }

    private async Task EnsureDaysSeededAsync(CancellationToken ct)
    {
        if (await OwnedDays().AnyAsync(ct))
        {
            return;
        }
        var templates = await _db.DietDays.AsNoTracking()
            .Where(d => d.UserId == null)
            .Include(d => d.Meals)
            .ToListAsync(ct);
        foreach (var t in templates)
        {
            var day = new DietDay
            {
                UserId = _me.Id,
                DayOfWeek = t.DayOfWeek,
                IsFastingDay = t.IsFastingDay,
                Notes = t.Notes,
            };
            foreach (var m in t.Meals.OrderBy(m => m.SortOrder))
            {
                day.Meals.Add(new Meal
                {
                    Slot = m.Slot,
                    TimeHint = m.TimeHint,
                    Items = m.Items,
                    ProteinG = m.ProteinG,
                    SortOrder = m.SortOrder,
                });
            }
            _db.DietDays.Add(day);
        }
        if (templates.Count > 0)
        {
            await _db.SaveChangesAsync(ct);
        }
    }

    private static void ApplyMeal(Meal meal, UpsertMealRequest request)
    {
        meal.Slot = Enum.TryParse<MealSlot>(request.Slot, true, out var slot) ? slot : MealSlot.Snack;
        meal.TimeHint = string.IsNullOrWhiteSpace(request.TimeHint) ? null : request.TimeHint.Trim();
        meal.Items = request.Items.Trim();
        meal.ProteinG = Math.Max(0, request.ProteinG);
    }

    private static NutritionProfileDto ToProfileDto(NutritionProfile p)
        => new(p.WeightKg, p.ProteinTargetG, p.CalorieTarget, p.DietStyle, p.Notes);

    private static DietDayDto ToDayDto(DietDay d)
        => new(
            d.Id,
            d.DayOfWeek,
            d.IsFastingDay,
            d.Notes,
            d.Meals.Sum(m => m.ProteinG),
            d.Meals.OrderBy(m => m.SortOrder).Select(m => new MealDto(m.Id, m.Slot.ToString(), m.TimeHint, m.Items, m.ProteinG, m.SortOrder)).ToList());
}
